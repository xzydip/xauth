# Vercel entrypoint: wraps the existing stdlib Handler from server.py.
import os
import sys
from urllib.parse import urlparse, parse_qsl, urlencode

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

import server  # noqa: E402

try:
    server.db_init()
except Exception as e:  # don't crash the function on cold start; log it
    print("db_init failed:", e)

# headers Vercel may use to carry the original URL
_ORIG_HEADERS = ("x-vercel-original-path", "x-invoke-path", "x-forwarded-uri",
                 "x-original-url", "x-original-uri")


def _restore_path(path, headers=None):
    """vercel.json rewrites every URL to /api/index?_p=<original path>.
    Rebuild the original path + query so server.Handler routes normally."""
    u = urlparse(path)
    pairs = parse_qsl(u.query, keep_blank_values=True)
    orig = None
    rest = []
    for k, v in pairs:
        if k == "_p" and orig is None:
            orig = v
        else:
            rest.append((k, v))
    if orig is None:
        if u.path in ("/api/index", "/api/index.py"):
            for h in _ORIG_HEADERS:
                hv = headers.get(h) if headers else None
                if hv and not hv.startswith("/api/index"):
                    return hv if hv.startswith("/") else "/" + hv
            orig = "/"
        else:
            return path  # path already original
    if not orig.startswith("/"):
        orig = "/" + orig
    q = urlencode(rest)
    return orig + ("?" + q if q else "")


class handler(server.Handler):
    _raw = ""

    def _prep(self):
        self._raw = self.path
        self.path = _restore_path(self.path, self.headers)

    def send_error(self, code, message=None, explain=None):
        # show what the function actually received (debug aid on 404s)
        if code == 404:
            message = "%s [raw=%s | now=%s]" % (message or "Not found", self._raw, self.path)
        return super().send_error(code, message, explain)

    def do_GET(self):
        self._prep()
        return super().do_GET()

    def do_POST(self):
        self._prep()
        return super().do_POST()

    def do_DELETE(self):
        self._prep()
        return super().do_DELETE()

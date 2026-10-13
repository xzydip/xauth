import os
import sys
from urllib.parse import urlparse, parse_qsl, urlencode

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

import server

try:
    server.db_init()
except Exception as e:
    print("db_init failed:", e)


def _restore_path(path):
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
            orig = "/"
        else:
            return path
    if not orig.startswith("/"):
        orig = "/" + orig
    q = urlencode(rest)
    return orig + ("?" + q if q else "")


class handler(server.Handler):
    def do_GET(self):
        self.path = _restore_path(self.path)
        return super().do_GET()

    def do_POST(self):
        self.path = _restore_path(self.path)
        return super().do_POST()

    def do_DELETE(self):
        self.path = _restore_path(self.path)
        return super().do_DELETE()

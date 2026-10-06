apt update -y
apt install -y python3 python3-pip unzip

git clone https://github.com/xzydip/xauth.git

unzip XZY_Auth_Final.zip
nohup python3 server.py > xauth.log 2>&1 &



For check ss -lntp | grep 3000



Your Auth Live http://localhost:3000

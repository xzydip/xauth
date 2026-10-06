apt update -y
apt install -y python3 python3-pip unzip

git clone https://github.com/xzydip/xauth.git

cd xauth

unzip xauth.zip

nohup python3 server.py > xauth.log 2>&1 &



For check ss -lntp | grep 3000



Your Auth Live http://localhost:3000

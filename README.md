apt update -y
apt install -y python3 python3-pip unzip

git clone https://github.com/xzydip/xauth.git

cd xauth

unzip xauth.zip

cd XzyAuth

python3 -m venv venv

source venv/bin/activate

pip install -r requirements.txt

python3 server.py






Your Auth Live http://localhost:3000

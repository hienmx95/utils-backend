#!/bin/bash
# sed -i "s/{SECRET_KEY}/${SECRET_KEY}/g" appsettings.json
# sed -i "s/{SQL_IP}/${SQL_IP}/g" appsettings.json
# sed -i "s/{SQL_DB}/${SQL_DB}/g" appsettings.json
# sed -i "s/{SQL_USER}/${SQL_USER}/g" appsettings.json
# sed -i "s/{SQL_PASS}/${SQL_PASS}/g" appsettings.json
# sed -i "s/{MG_IP}/${MG_IP}/g" appsettings.json
# sed -i "s/{MG_USER}/${MG_USER}/g" appsettings.json
# sed -i "s/{MG_PASS}/${MG_PASS}/g" appsettings.json
# sed -i "s/{FB_CREDENTIAL}/${FB_CREDENTIAL}/g" appsettings.json
# sed -i "s/{FACEBOOK_APPID}/${FACEBOOK_APPID}/g" appsettings.json
# sed -i "s/{FACEBOOK_APPSECRET}/${FACEBOOK_APPSECRET}/g" appsettings.json
# sed -i "s/{PUBLIC_RSA_KEY}/${PUBLIC_RSA_KEY}/g" appsettings.json
# sed -i "s/{EMAIL_ADDRESS}/${EMAIL_ADDRESS}/g" appsettings.json
# sed -i "s/{EMAIL_SERVER}/${EMAIL_SERVER}/g" appsettings.json
# sed -i "s/{EMAIL_PORT}/${EMAIL_PORT}/g" appsettings.json
# sed -i "s/{EMAIL_USER}/${EMAIL_USER}/g" appsettings.json
# sed -i "s/{EMAIL_PASSWD}/${EMAIL_PASSWD}/g" appsettings.json

PROJECT_NAME="Utils"
dotnet ${PROJECT_NAME}.dll --urls http://0.0.0.0:8080 --launch-profile ${MODE}
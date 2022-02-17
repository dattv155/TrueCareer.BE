#!/bin/bash
sed -i "s/{SECRET_KEY}/${SECRET_KEY}/g" appsettings.json
sed -i "s/{EXPIRED_TIME}/${EXPIRED_TIME}/g" appsettings.json
sed -i "s/{ORGANIZATION_ROOT}/${ORGANIZATION_ROOT}/g" appsettings.json
sed -i "s/{ADMIN_PASS}/${ADMIN_PASS}/g" appsettings.json
sed -i "s/{SQL_IP}/${SQL_IP}/g" appsettings.json
sed -i "s/{SQL_DB}/${SQL_DB}/g" appsettings.json
sed -i "s/{SQL_USER}/${SQL_USER}/g" appsettings.json
sed -i "s/{SQL_PASS}/${SQL_PASS}/g" appsettings.json
sed -i "s/{PRIVATE_RSA_KEY}/${PRIVATE_RSA_KEY}/g" appsettings.json
sed -i "s/{PUBLIC_RSA_KEY}/${PUBLIC_RSA_KEY}/g" appsettings.json
sed -i "s/{LDAP_IP}/${LDAP_IP}/g" appsettings.json
sed -i "s/{REDIS_ENDPOINT}/${REDIS_ENDPOINT}/g" appsettings.json

PROJECT_NAME="TrueCareer.BE"

dotnet ${PROJECT_NAME}.dll --urls http://0.0.0.0:8080 --launch-profile ${MODE}


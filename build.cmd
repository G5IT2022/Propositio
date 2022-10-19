@echo off


docker run --rm --env "TZ=Europe/Oslo" --name mariadb -p 3306:3306/tcp  -e MYSQL_ROOT_PASSWORD=12345 -d mariadb:latest

docker cp "%cd%\Databasefiler\setupDB.sql" mariadb:"bin"



:: Kill running instance of container
docker kill webapp

:: Builds image specified in Dockerfile
docker image build -t webapp .

:: Starts container with web application and maps port 80 (ext) to 80 (internal)
docker container run --rm -it -d --name webapp --publish 4000:4000 webapp

echo.
echo "Link: http://localhost:4000/"
echo.

docker exec -it mariadb bash
pause

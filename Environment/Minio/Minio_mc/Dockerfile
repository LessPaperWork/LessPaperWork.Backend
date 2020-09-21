FROM ubuntu:latest

RUN apt update && \
    apt upgrade -y && \
    apt install wget -y && \
    wget -O /tmp/mc  https://dl.min.io/client/mc/release/linux-amd64/mc && \
    chmod +x /tmp/mc
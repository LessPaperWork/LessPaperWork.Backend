{ pkgs ? import <nixpkgs> { 
    config.allowUnfree = true; 
  }
}:
pkgs.mkShell {
  name = "lesspaper-shell";
  buildInputs = [
    pkgs.git
    pkgs.dotnet-sdk_3
    pkgs.vscodium
    pkgs.jetbrains.rider
    pkgs.mongodb-compass
    pkgs.docker-compose
  ];
  shellHook = ''
    codium . &
    rider ../LessPaperWork.Backend.sln &
    mongodb-compass &

    docker-compose -f ../Environment/Minio/docker-compose.yml up &
    docker-compose -f ../Environment/MongoDB/docker-compose.yml up &
    docker-compose -f ../Environment/RabbitMq/docker-compose.yml up &
  ''; 
}
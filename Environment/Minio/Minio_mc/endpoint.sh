cp /root/TestCertificates/*.crt /usr/local/share/ca-certificates/ 
update-ca-certificates 

echo "Sleep for "$MINIO_MC_SLEEP_SEC" seconds"
sleep $MINIO_MC_SLEEP_SEC"s"

/tmp/mc config host add minio https://minio:9000 $MINIO_ACCESS_KEY $MINIO_SECRET_KEY
/tmp/mc mb minio/lesspaper
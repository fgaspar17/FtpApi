FROM alpine:3.14

RUN apk add --no-cache vsftpd=3.0.4-r0

COPY vsftpd.conf /etc/vsftpd/vsftpd.conf

# Create home directory for vsftpd
RUN mkdir -p /home/vsftpd/
RUN chown -R ftp:ftp /home/vsftpd/

# Create log directory
RUN mkdir -p /var/log/vsftpd

# Create the user, set password, and configure the home directory
RUN adduser ftpapi;echo 'ftpapi:123' | chpasswd

# Expose necessary FTP ports
EXPOSE 21 990 5000-5100

CMD ["/usr/sbin/vsftpd", "/etc/vsftpd/vsftpd.conf"]

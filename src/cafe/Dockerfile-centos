FROM centos

MAINTAINER Annie Hedgpeth <annie.hedgpeth@gmail.com>

COPY bin/Debug/netcoreapp1.1/centos.7-x64/publish /usr/bin/cafe/

EXPOSE 59320

ENTRYPOINT ["/usr/bin/cafe/cafe"]
CMD ["server"]
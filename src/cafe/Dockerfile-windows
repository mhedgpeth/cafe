FROM microsoft/windowsservercore

MAINTAINER Annie Hedgpeth <annie.hedgpeth@gmail.com>

COPY bin/Debug/netcoreapp1.1/win10-x64/publish C:/cafe/

EXPOSE 59320

ENTRYPOINT ["C:/cafe/cafe.exe"]
CMD ["server"]
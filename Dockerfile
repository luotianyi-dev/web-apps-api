FROM        mcr.microsoft.com/dotnet/sdk:8.0 AS builder
COPY        . /src
WORKDIR       /src
RUN         dotnet publish "AppsApi.csproj" -c Release -o /app -r linux-musl-x64 --nologo --sc true /p:PublishSingleFile=true /p:DebugType=None /p:DebugSymbols=false


FROM        alpine:3.18
ARG         IMAGE_VERSION=latest
LABEL       org.opencontainers.image.title=apps-api-server \
            org.opencontainers.image.url=https://github.com/luotianyi-dev/web-apps-api \
            org.opencontainers.image.source=git+https://github.com/luotianyi-dev/web-apps-api.git \
            org.opencontainers.image.documentation=https://github.com/luotianyi-dev/web-apps-api/blob/main/README.md \
            org.opencontainers.image.licenses=MPL-2.0 \
            org.opencontainers.image.version=${IMAGE_VERSION}
COPY        --from=builder /app /app
WORKDIR     /app
RUN         apk add --no-cache curl icu-libs && \
                chown -R root:root /app && \
                ldd /app/apps-api-server
EXPOSE      5000/tcp
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=5 \
                CMD curl -H "User-Agent: Health-Check" -f http://127.0.0.1:5000/healthz || exit 1
ENTRYPOINT  ["/app/apps-api-server", "--urls=http://*:5000/"]

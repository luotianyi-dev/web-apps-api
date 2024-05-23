# Tianyi Network Apps API Server

Tianyi Network Apps API Server 是 Tianyi Network Apps 的后端 API 实现，基于 [ASP.NET Core 8](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0) 和 [MariaDB 15+](https://mariadb.org/) 开发。

 - 命名空间：`TianyiNetwork.Web.AppsApi`
 - 解决方案名称: `AppsApi`
 - 构建二进制名称：`apps-api-server`
 - Docker 镜像名称：`ghcr.io/luotianyi-dev/apps-api-server`

[![ASP.NET Core 8](https://img.shields.io/badge/ASP.NET_Core-8.0_LTS-512bd4?style=flat-square&logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0)
[![MariaDB 15+](https://img.shields.io/badge/MariaDB-15%2B-orange?style=flat-square&logo=mariadb&logoColor=white)](https://mariadb.org/)
[![GitHub Actions: Build](https://img.shields.io/github/actions/workflow/status/luotianyi-dev/web-apps-api/build.yml?style=flat-square&logo=github&logoColor=white&label=Build)](https://github.com/luotianyi-dev/web-apps-api/actions/workflows/build.yml)
[![GitHub Actions: Release](https://img.shields.io/github/actions/workflow/status/luotianyi-dev/web-apps-api/release.yml?style=flat-square&logo=github&logoColor=white&label=Release)](https://github.com/luotianyi-dev/web-apps-api/actions/workflows/release.yml)
[![ghcr.io: luotianyi-dev/apps-api-server](https://img.shields.io/badge/ghcr.io-luotianyi--dev%2Fapps--api--server-blue?style=flat-square)](https://github.com/orgs/luotianyi-dev/packages/container/package/apps-api-server)
[![license: MPL-2.0](https://img.shields.io/github/license/luotianyi-dev/web-apps-api?style=flat-square&label=license&color=blue)](https://github.com/luotianyi-dev/web-apps-api/blob/main/LICENSE)
[![Latest Release](https://img.shields.io/github/v/release/luotianyi-dev/web-apps-api?style=flat-square&color=ee82ee)](https://github.com/luotianyi-dev/web-apps-api/releases/latest)

Tianyi Network Apps API Server 是 Tianyi Network Web 的一部分。

## 实现的功能
请参见 [Tianyi Network Apps Web](https://github.com/luotianyi-dev/web-apps) 的文档。

## 构建
Tianyi Network Apps API Server 基于 [ASP.NET 8](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0) 框架开发。您需要安装 .NET 8 SDK 来构建此项目。

安装完成后，您可以使用以下命令构建项目：
```shell
dotnet build
```

若您有权限访问内容安全库 (Tianyi Network Apps Content Security Libaray)，请在生产构建前下载并构建内容安全库：
```shell
gh repo clone luotianyi-dev/web-apps-content-security
python3 web-apps-content-security/blocked-words/main.py
mv web-apps-content-security/blocked-words/dist/blocked-words.lst \
    ./Resources/BlockedWord/BlockedWords.txt
cat ./Resources/BlockedWord/BlockedWords.txt | wc -l  # 统计行数
```

> 若您没有相关权限，请忽略此步骤。若需要运行上面的命令，您需要安装 [GitHub CLI](https://cli.github.com/)。

通过下面的命令构建生产版本的二进制 (用于 Linux)：
```shell
dotnet publish "AppsApi.csproj" -c Release -o bin/publish -r linux-musl-x64 --nologo --sc true /p:PublishSingleFile=true /p:DebugType=None /p:DebugSymbols=false
```

Tianyi Network Apps API Server 也支持 Docker 镜像构建。您可以使用以下命令构建 Docker 镜像：
```shell
docker build -t apps-api-server .
```

## 部署
### 在 Linux 上部署
要在 Linux 上部署，您需要首先安装以下依赖：
 - `libicu`: 在不同发行版上可能有不同的名称，请参见您的 Linux 发行版的文档安装之。
 - `mariadb`: 请安装 MariaDB 15.0 及以上版本。
Tianyi Network Apps API Server 默认使用 Self-contained 发布模式，因此您无需安装 ASP.NET 8 运行时，可以直接运行发布的二进制文件。

请确保您的二进制文件目录下包含以下文件或目录：
 - `Resources/`: 资源目录，包含应用程序所需的资源文件。
 - `appsettings.json`: 应用程序配置文件。

您需要根据业务需要配置数据库。并修改 `appsettings.json` 文件中的数据库连接字符串：
```json
{
  "ConnectionStrings": {
    "MySql": "Server=db-host.local;Port=3306;User=apps;Password=apps;Database=apps;"
  }
}
```

然后运行二进制文件：
```shell
./apps-api-server
```

您可以使用 Systemd 服务来管理应用程序。请参见 .NET 官方文档：[Host ASP.NET Core on Linux with Nginx](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0)

### 使用 Docker 部署
Tianyi Network Apps API Server 的 Docker 基于 `mcr.microsoft.com/dotnet/aspnet:8.0` 构建，并使用 [Alpine 3.18](https://alpinelinux.org/) 作为运行时环境。

镜像不包含 MariaDB。您需要使用 `docker-compose` 编排或在其他服务器上运行 MariaDB。

#### 基础镜像
此镜像基于 Alpine 3.18 ([`alpine:3.18`](https://hub.docker.com/_/alpine))。

#### 标签
 - `latest`: 最新的稳定版本。
 - `<git-version>`: 与 Git 标签对应的版本。
 - `1.0.0`

#### 运行镜像
运行以下命令以获取镜像：
```shell
docker pull ghcr.io/luotianyi-dev/apps-api-server
```

运行以下命令以启动容器：
```shell
docker run -d --name apps-api-server \
    -p 5000:5000 \
    -e "ConnectionStrings:MySql=Server=mysql-host.local;Port=3306;User=apps;Password=apps;Database=apps;" \
    ghcr.io/luotianyi-dev/apps-api-server
```
您需要将命令中的 `-e "ConnectionStrings:Mysql=..."` 替换为您的数据库连接字符串。

您可以将自己的内容安全配置文件挂载到 `/app/Resources/BlockedWord/BlockedWords.txt`：
```shell
docker run -d --name apps-api-server \
    -p 5000:5000 \
    -e "ConnectionStrings:MySql=Server=mysql-host.local;Port=3306;User=apps;Password=apps;Database=apps;" \
    -v /path/to/your/blocked-words.txt:/app/Resources/BlockedWord/BlockedWords.txt \
    ghcr.io/luotianyi-dev/apps-api-server
```

#### 配置
##### 环境变量
 - `ConnectionStrings:MySql`: 数据库连接字符串。

其他 ASP.NET Core Web Host 环境变量，请参见 [ASP.NET Core Web Host 文档](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-8.0#host-configuration-values)。

##### 卷
此镜像默认不使用卷。

您可以根据需要，将任意内容挂载到 `/app/Resources/{something}` 来覆盖默认资源文件，如字体文件或内容安全文件。

##### 暴露端口
此镜像默认暴露以下端口：
 - `5000/tcp`: ASP.NET HTTP API 端口。

Tianyi Network Apps API Server 在 `/apps/api` 路径下提供 API 服务。您需要合适的反向代理配置，及配套的前端应用程序来使用 API 服务。

##### 配置文件
配置文件应被挂载到 `/app/appsettings.json`，该文件应是合法的 ASP.NET Core AppSettings 配置文件。

您可以在 [appsettings.json](https://github.com/luotianyi-dev/web-apps-api/blob/main/appsettings.json) 中找到示例配置文件。

#### 健康检查
此镜像默认启用健康检查。您无需在 Docker 运行命令或 Docker Compose 文件中配置健康检查。

此镜像在 `/healthz` 路径下提供健康检查服务。您可以与第三方应用程序集成以检查应用程序的健康状态。如果应用程序正常工作，将返回 `200 OK` 状态码和 `Healthy` 字符串：
```bash
$ curl -SsfL http://127.0.0.1:5000/healthz
Healthy
```

此镜像使用的健康检查命令为：
```shell
curl -H "User-Agent: Health-Check" -f http://127.0.0.1:5000/healthz || exit 1
```

#### 使用 Docker Compose 运行
以下为 Docker Compose 配置文件示例：
```yaml
networks:
  app:
    name: apps
    driver_opts:
      com.docker.network.bridge.name: cni-br0
    ipam:
      config:
        - subnet: 192.168.18.0/24

services:
  api:
    image: ghcr.io/luotianyi-dev/apps-api-server:latest
    container_name: apps-api-server
    restart: always
    networks:
      app:
        ipv4_address: 192.168.18.2
    environment:
      - "ConnectionStrings:MySql=Server=mysql;Port=3306;User=apps;Password=apps;Database=apps;"
    ports:
      - "127.0.0.1:5000:5000"
    deploy:
      resources:
        limits:
          memory: 512M
  mysql:
    image: mariadb:latest
    container_name: apps-mysql
    restart: always
    networks:
      app:
        ipv4_address: 192.168.18.3
    environment:
    - "MYSQL_ROOT_PASSWORD=root"
    - "MYSQL_DATABASE=apps"
    - "MYSQL_USER=apps"
    - "MYSQL_PASSWORD=apps"
    volumes:
    - /srv/apps/mysql:/var/lib/mysql
```

## 开发
只要您安装了 .NET SDK 开发环境，您可以使用 Visual Studio 2022 以上版本、JetBrains Rider 或 Visual Studio Code 来开发 Tianyi Network Apps API Server。

您可以使用以下命令获取源代码：
```shell
git clone https://github.com/luotianyi-dev/web-apps-api.git
```

然后尝试构建：
```shell
dotnet build -c Debug
```

您可以用您偏好的 IDE 打开 `AppsApi.sln` 来打开解决方案，以获取更好的开发体验。

## 许可
Tianyi Network Apps API Server 使用 [Mozilla Public License 2.0](https://www.mozilla.org/en-US/MPL/2.0/) 许可。

请注意，此项目部分资源文件 (`Resources/` 文件夹下的文件) 可能来自第三方，其许可可能不同于本项目。其许可可能是非商业使用、SIL Open Font License、或其他开源或非开源的许可。

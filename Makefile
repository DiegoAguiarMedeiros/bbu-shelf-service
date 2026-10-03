# ==============================================================================
# BBU User Service – EF Core Migration Helpers
# ==============================================================================
# Usage:
#   make migration NAME=InitialCreate   – create a new migration
#   make db-update                      – apply pending migrations
#   make migration-remove               – remove the last migration
#   make migration-list                 – list all migrations
#   make migration-script               – generate idempotent SQL script
#   make restore                        – restore NuGet packages
#   make build                          – build the solution
#   make run                            – run the Api project
# ==============================================================================
NETWORK        := bbu_bbu_network
SDK_IMAGE      := mcr.microsoft.com/dotnet/sdk:10.0
INFRASTRUCTURE := src/Infrastructure/Infrastructure.csproj
STARTUP        := src/Api/Api.csproj
API            := src/Api/Api.csproj
DB_CONN        := Host=bbu-user-postgres;Database=ShelfDb;Username=postgres;Password=postgres
NAME ?= Migration

define EF
	docker run --rm \
		-v "$(PWD)":/src \
		-w /src \
		--network $(NETWORK) \
		$(SDK_IMAGE) \
		bash -lc " \
			export PATH=/root/.dotnet/tools:\$$PATH; \
			dotnet tool install --global dotnet-ef --version 9.* 2>/dev/null || true; \
			$(1)"
endef

define DOTNET
	docker run --rm \
		-v "$(PWD)":/src \
		-w /src \
		--network $(NETWORK) \
		$(SDK_IMAGE) \
		bash -lc "$(1)"
endef

.PHONY: migration db-update migration-remove migration-list migration-script restore build run help

## make migration NAME=MinhasMigration  –  cria uma nova migration
migration:
	$(call EF, dotnet ef migrations add $(NAME) \
		--project $(INFRASTRUCTURE) \
		--startup-project $(STARTUP))

## make db-update  –  aplica as migrations pendentes
db-update:
	$(call EF, dotnet ef database update \
		--project $(INFRASTRUCTURE) \
		--startup-project $(STARTUP) \
		--connection '$(DB_CONN)')

## make migration-remove  –  remove a última migration
migration-remove:
	$(call EF, dotnet ef migrations remove \
		--project $(INFRASTRUCTURE) \
		--startup-project $(STARTUP))

## make migration-list  –  lista todas as migrations
migration-list:
	$(call EF, dotnet ef migrations list \
		--project $(INFRASTRUCTURE) \
		--startup-project $(STARTUP) \
		--connection '$(DB_CONN)')

## make migration-script  –  gera script SQL idempotente (migrations.sql)
migration-script:
	$(call EF, dotnet ef migrations script --idempotent \
		--project $(INFRASTRUCTURE) \
		--startup-project $(STARTUP) \
		--output migrations.sql)

## make restore  –  restaura os pacotes NuGet do projeto Api (e dependências)
restore:
	$(call DOTNET, dotnet restore $(API))

## make build  –  builda o projeto Api (e dependências)
build:
	$(call DOTNET, dotnet build $(API))

## make run  –  roda a Api localmente (dentro do container)
run:
	$(call DOTNET, dotnet run --project $(API))

## make run  –  roda a Api localmente (dentro do container, apenas HTTP)
run:
	docker run --rm \
		-v "$(PWD)":/src \
		-w /src \
		-p 5148:5148 \
		-e ASPNETCORE_URLS=http://+:5148 \
		--network $(NETWORK) \
		$(SDK_IMAGE) \
		bash -lc "dotnet run --project $(API) --no-launch-profile"
## make help  –  exibe os targets disponíveis
help:
	@grep -E '^##' Makefile | sed 's/## //'
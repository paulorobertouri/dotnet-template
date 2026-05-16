.PHONY: help install build run test test-unit test-e2e lint format clean

help:
	@echo "Available commands:"
	@echo "  install      Restore dependencies"
	@echo "  build        Build the solution"
	@echo "  run          Run the API project"
	@echo "  test         Run all tests"
	@echo "  test-unit    Run unit tests"
	@echo "  test-e2e     Run E2E tests with Playwright"
	@echo "  lint         Run dotnet format check"
	@echo "  format       Run dotnet format"
	@echo "  clean        Clean build artifacts"

install:
	dotnet restore

build:
	dotnet build --configuration Release

run:
	dotnet run --project src/DotnetTemplate.Api

start:
	bash ./scripts/ubuntu/start.sh

stop:
	bash ./scripts/ubuntu/stop.sh

test:
	dotnet test

test-unit:
	dotnet test tests/DotnetTemplate.Tests.Unit

test-e2e:
	# Requires node/playwright installed
	npm exec playwright test

lint:
	dotnet format --verify-no-changes

format:
	dotnet format

clean:
	dotnet clean
	rm -rf src/*/bin src/*/obj tests/*/bin tests/*/obj

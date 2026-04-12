.PHONY: install run test test-coverage format lint docker-build docker-test docker-curl-test clean help

install:
	bash ./scripts/ubuntu/install.sh

run:
	bash ./scripts/ubuntu/run.sh

test:
	bash ./scripts/ubuntu/test.sh

test-coverage:
	dotnet test DotnetTemplate.sln \
		--collect:"XPlat Code Coverage" \
		--results-directory ./coverage

format:
	bash ./scripts/ubuntu/format.sh

lint:
	bash ./scripts/ubuntu/lint.sh

docker-build:
	docker build -f docker/build.Dockerfile -t dotnet-template:latest .

docker-test:
	docker build -f docker/test.Dockerfile -t dotnet-template-test:latest .
	docker run --rm dotnet-template-test:latest

docker-curl-test:
	bash ./scripts/ubuntu/docker-curl-test.sh

clean:
	dotnet clean DotnetTemplate.sln
	find . -type d -name bin  -not -path "*/\.*" | xargs rm -rf
	find . -type d -name obj  -not -path "*/\.*" | xargs rm -rf
	rm -rf coverage

help:
	@echo "Available targets:"
	@echo "  install          Restore NuGet packages"
	@echo "  run              Run the API (Development)"
	@echo "  test             Run all tests"
	@echo "  test-coverage    Run tests with coverage report"
	@echo "  format           Apply dotnet format"
	@echo "  lint             Verify formatting (no changes)"
	@echo "  docker-build     Build production Docker image"
	@echo "  docker-test      Build and run tests inside Docker"
	@echo "  docker-curl-test Build image, start container, run curl tests"
	@echo "  clean            Remove build artifacts"

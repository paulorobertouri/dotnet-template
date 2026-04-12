# Customizing the dotnet-template

This template is designed for easy renaming and project-specific setup.

## 1. Rename the Project

- Update all references to `dotnet-template` and `DotnetTemplate` in:
  - `DotnetTemplate.sln`
  - `src/**/*.csproj`
  - `README.md` and related docs
  - `.github/workflows/*`

- Or run the provided rename scripts:

  ```bash
  ./scripts/rename-template.sh my-new-project
  ```

  ```powershell
  .\scripts\rename-template.ps1 my-new-project
  ```

## 2. Update Metadata

- Replace repository URLs and badge links in `README.md`.
- Update assembly names, namespaces, and any package metadata if needed.

## 3. Review Configuration

- Copy `.env.example` to `.env`.
- Set a strong `JWT_SECRET` before local or container runs.
- Review issuer, audience, and expiration settings for your environment.

## 4. Review CI/CD

- Update `.github/workflows/ci.yml` to match your repository and branch policy.
- Replace placeholder org or repo names in badges and workflow references.

## 5. Review Security and Ownership

- Update `SECURITY.md` with the real reporting address.
- Update `CODEOWNERS` with the maintainers for the new project.

## 6. Remove Template Helpers

- Delete the rename scripts after the project has been renamed if you do not want to keep them.

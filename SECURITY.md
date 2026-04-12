# Security Policy

## Supported Versions

| Version | Supported |
|---------|-----------|
| latest  | ✅        |

## Reporting a Vulnerability

Please **do not** open a public GitHub issue for security vulnerabilities.

Send a private report to: **security@example.com**
(replace with your project e-mail before publishing)

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (optional)

You will receive an acknowledgement within **48 hours** and a resolution plan within **7 days**.

## Security Hardening Notes

- `JWT_SECRET` must be at minimum 32 characters. The app refuses to start if it is empty.
- Never commit a `.env` file — it is listed in `.gitignore`.
- The Docker image runs as a non-root user (`appuser`) on port 8080.
- In production, set `JWT_SECRET` via an environment variable or secrets manager — never via `appsettings.json`.

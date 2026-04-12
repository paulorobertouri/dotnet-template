#!/usr/bin/env bash
# Usage: ./rename-template.sh MyNewProject
# Renames every occurrence of "DotnetTemplate" and "dotnet-template" to your project name.
set -euo pipefail

if [ -z "${1:-}" ]; then
  echo "Usage: $0 MyNewProject"
  exit 1
fi

NEW_NAME="$1"
PASCAL="$(echo "$NEW_NAME" | sed 's/-\([a-z]\)/\u\1/g; s/^\([a-z]\)/\u\1/g')"
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT"

echo "Renaming project to: $NEW_NAME (Pascal: $PASCAL)"

# Replace in file contents
find . \
  -not -path '*/.git/*' \
  -not -path '*/bin/*' \
  -not -path '*/obj/*' \
  -not -path '*/node_modules/*' \
  -type f \
  \( -name '*.cs' -o -name '*.csproj' -o -name '*.sln' \
     -o -name '*.json' -o -name '*.yml' -o -name '*.yaml' \
     -o -name '*.md' -o -name 'Makefile' -o -name 'Dockerfile' \
     -o -name '*.sh' -o -name '*.ps1' \) \
  -exec sed -i \
    -e "s/DotnetTemplate/${PASCAL}/g" \
    -e "s/dotnet-template/${NEW_NAME}/g" \
    {} +

# Rename directories containing "DotnetTemplate"
while IFS= read -r -d '' dir; do
  new_dir="${dir//DotnetTemplate/$PASCAL}"
  if [ "$dir" != "$new_dir" ]; then
    mv "$dir" "$new_dir"
    echo "  Renamed dir: $dir -> $new_dir"
  fi
done < <(find . \
  -not -path '*/.git/*' \
  -not -path '*/bin/*' \
  -not -path '*/obj/*' \
  -depth -type d -name "*DotnetTemplate*" -print0)

# Rename files containing "DotnetTemplate"
while IFS= read -r -d '' file; do
  dir="$(dirname "$file")"
  base="$(basename "$file")"
  new_base="${base//DotnetTemplate/$PASCAL}"
  if [ "$base" != "$new_base" ]; then
    mv "$file" "$dir/$new_base"
    echo "  Renamed file: $file -> $dir/$new_base"
  fi
done < <(find . \
  -not -path '*/.git/*' \
  -not -path '*/bin/*' \
  -not -path '*/obj/*' \
  -type f -name "*DotnetTemplate*" -print0)

echo ""
echo "Done. Verify the solution still builds:"
echo "  dotnet build ${PASCAL}.sln"

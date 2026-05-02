#!/usr/bin/env bash
# ─────────────────────────────────────────────────────────────────────────────
# push-to-github.sh
# Pousse les 3 branches netIPAM2 vers https://github.com/AnthoDingo/netIPAM2
#
# Usage :
#   export GITHUB_TOKEN=ghp_XXXXXXXXXXXXXXXXXXXX
#   bash push-to-github.sh
#
# Générez un token sur : https://github.com/settings/tokens
# Scope requis         : repo  (ou public_repo si le dépôt est public)
# ─────────────────────────────────────────────────────────────────────────────
set -e

REPO="https://github.com/AnthoDingo/netIPAM2.git"
BRANCHES=("feature/v1-foundation" "feature/v2-admin" "feature/v3-passkeys")

if [ -z "$GITHUB_TOKEN" ]; then
  echo "❌  Variable GITHUB_TOKEN non définie."
  echo "    Exportez-la avant de lancer ce script :"
  echo "    export GITHUB_TOKEN=ghp_XXXXXXXXXXXXXXXXXXXX"
  exit 1
fi

REMOTE_AUTH="https://${GITHUB_TOKEN}@github.com/AnthoDingo/netIPAM2.git"

# S'assurer qu'on est dans le bon dossier
cd "$(dirname "$0")"

# Configurer le remote avec auth (temporaire, ne va pas dans .git/config en clair)
git remote set-url origin "$REMOTE_AUTH"

echo "📤  Push des branches vers $REPO"
for branch in "${BRANCHES[@]}"; do
  echo ""
  echo "→  $branch"
  git push origin "$branch" --force-with-lease
  echo "   ✓ OK"
done

# Remettre l'URL sans token
git remote set-url origin "$REPO"

echo ""
echo "✅  Terminé. Branches disponibles sur :"
echo "    https://github.com/AnthoDingo/netIPAM2/branches"
echo ""
echo "Pour créer des Pull Requests :"
echo "  v1 → main : https://github.com/AnthoDingo/netIPAM2/compare/main...feature/v1-foundation"
echo "  v2 → v1   : https://github.com/AnthoDingo/netIPAM2/compare/feature/v1-foundation...feature/v2-admin"
echo "  v3 → v2   : https://github.com/AnthoDingo/netIPAM2/compare/feature/v2-admin...feature/v3-passkeys"

# Guide d'Implémentation - Refactorisation Razor

## 📋 Table des matières
1. [Instructions pour la Phase 1](#phase-1-pages-subnets)
2. [Checklist de Vérification](#-checklist-de-vérification)
3. [Dépannage Courant](#-dépannage-courant)
4. [Validation Finale](#-validation-finale)

---

## 🚀 Phase 1: Pages/Subnets

### Étape 1: Sauvegarder les fichiers originaux
```bash
cd netIPAM/Components/Pages/Subnets

# Créer une branche de sauvegarde
git checkout -b backup/subnets-original

# Vérifier le statut
git status
```

### Étape 2: Copier les fichiers refactorisés

Remplacer les 4 fichiers .razor:

```bash
# Depuis le répertoire du projet
cp /home/claude/REFACTORED_FILES/Subnets/Edit.razor ./netIPAM/Components/Pages/Subnets/
cp /home/claude/REFACTORED_FILES/Subnets/View.razor ./netIPAM/Components/Pages/Subnets/
cp /home/claude/REFACTORED_FILES/Subnets/IpEdit.razor ./netIPAM/Components/Pages/Subnets/
cp /home/claude/REFACTORED_FILES/Subnets/SectionSubnets.razor ./netIPAM/Components/Pages/Subnets/
```

### Étape 3: Ajouter les fichiers .razor.cs

Créer 4 nouveaux fichiers:

```bash
cp /home/claude/REFACTORED_FILES/Subnets/Edit.razor.cs ./netIPAM/Components/Pages/Subnets/
cp /home/claude/REFACTORED_FILES/Subnets/View.razor.cs ./netIPAM/Components/Pages/Subnets/
cp /home/claude/REFACTORED_FILES/Subnets/IpEdit.razor.cs ./netIPAM/Components/Pages/Subnets/
cp /home/claude/REFACTORED_FILES/Subnets/SectionSubnets.razor.cs ./netIPAM/Components/Pages/Subnets/
```

### Étape 4: Vérifier la structure

```bash
ls -la netIPAM/Components/Pages/Subnets/
```

Vous devriez voir:
```
├── Edit.razor
├── Edit.razor.cs          ✅ NEW
├── View.razor
├── View.razor.cs          ✅ NEW
├── IpEdit.razor
├── IpEdit.razor.cs        ✅ NEW
├── SectionSubnets.razor
└── SectionSubnets.razor.cs ✅ NEW
```

### Étape 5: Compiler et Tester

```bash
# Compiler le projet
dotnet build

# Lancer les tests (si existants)
dotnet test

# Lancer l'application
dotnet run
```

---

## ✅ Checklist de Vérification

### Avant de Compiler

- [ ] **Namespaces Corrects**
  ```bash
  grep -n "namespace netIPAM.Components.Pages.Subnets" \
    netIPAM/Components/Pages/Subnets/*.razor.cs
  ```
  Tous les fichiers doivent avoir: `namespace netIPAM.Components.Pages.Subnets`

- [ ] **Classe Partielle**
  ```bash
  grep "public partial class" netIPAM/Components/Pages/Subnets/*.razor.cs
  ```
  Doit afficher 4 lignes (Edit, View, IpEdit, SectionSubnets)

- [ ] **Injection de Dépendances**
  ```bash
  grep -c "\[Inject\]" netIPAM/Components/Pages/Subnets/*.razor.cs
  ```
  Doit être > 0 pour chaque fichier approprié

- [ ] **Pas de @code dans .razor**
  ```bash
  grep "@code" netIPAM/Components/Pages/Subnets/*.razor
  ```
  ❌ Ne doit rien afficher! (0 résultats)

### Après Compilation

- [ ] ✅ **Le projet compile sans erreurs**
  ```bash
  dotnet build 2>&1 | grep -i error
  ```

- [ ] ✅ **Pas de warnings Razor**
  ```bash
  dotnet build 2>&1 | grep -i "razor"
  ```

### Après le Déploiement

- [ ] **Les pages Subnets se chargent correctement**
  - Aller sur `/sections/1/subnets` (adapter le numéro)
  - Vérifier l'affichage des subnets
  - Cliquer sur un subnet → vérifier la page de détail

- [ ] **Les opérations CRUD fonctionnent**
  - Créer un subnet (new)
  - Éditer un subnet (edit)
  - Supprimer un subnet (delete)
  - Créer une IP
  - Éditer une IP
  - Supprimer une IP

---

## 🔍 Dépannage Courant

### Erreur 1: "Component type not found"
**Symptôme**: Le navigateur affiche une erreur de composant non trouvé.

**Cause**: Le fichier `.razor.cs` n'a pas le bon nom de classe.

**Solution**:
```bash
# Vérifier le nom de classe vs nom de fichier
# Fichier: Edit.razor → Classe: public partial class Edit ✅
# Fichier: View.razor → Classe: public partial class View ✅
# Fichier: IpEdit.razor → Classe: public partial class IpEdit ✅
# Fichier: SectionSubnets.razor → Classe: public partial class SectionSubnets ✅
```

### Erreur 2: "The property X does not exist on type"
**Symptôme**: Erreur de compilation genre "property '_subnet' does not exist"

**Cause**: Propriétés privées non déclarées dans `.razor.cs`

**Solution**: Vérifier que le fichier `.razor.cs` contient:
```csharp
private Subnet? _subnet;       // Pour Edit & View
private IpAddress? _ip;        // Pour IpEdit
private Section? _section;     // Pour SectionSubnets
private List<Subnet>? _subnets; // Pour SectionSubnets
```

### Erreur 3: "Unknown attribute '@inject'"
**Symptôme**: L'IDE souligne `@inject` comme erreur

**Cause**: Dans Razor, il faut utiliser `[Inject]` dans `.razor.cs`, pas `@inject` dans `.razor`

**Solution**: Les fichiers `.razor` ne doivent PAS avoir:
```razor
❌ @inject SubnetService Subnets
```

Ils doivent être dans `.razor.cs`:
```csharp
✅ [Inject]
   private SubnetService Subnets { get; set; } = default!;
```

### Erreur 4: "The property 'RenderSubnetRow' does not exist"
**Symptôme**: Erreur lors du rendu de `SectionSubnets.razor`

**Cause**: La méthode `RenderSubnetRow` n'est pas déclarée

**Solution**: Vérifier que `SectionSubnets.razor.cs` contient la méthode `RenderSubnetRow`

---

## ✨ Validation Finale

### Script de Vérification Complet

```bash
#!/bin/bash
# verify_refactor.sh

SUBNETS_DIR="netIPAM/Components/Pages/Subnets"

echo "🔍 Vérification de la refactorisation..."

# 1. Vérifier les fichiers .razor.cs existent
echo ""
echo "1️⃣  Fichiers .razor.cs:"
for file in Edit View IpEdit SectionSubnets; do
    if [ -f "$SUBNETS_DIR/$file.razor.cs" ]; then
        echo "  ✅ $file.razor.cs"
    else
        echo "  ❌ $file.razor.cs (MANQUANT!)"
    fi
done

# 2. Vérifier pas de @code dans .razor
echo ""
echo "2️⃣  Blocs @code dans .razor:"
if grep -r "@code" "$SUBNETS_DIR"/*.razor; then
    echo "  ❌ ERREUR: Des blocs @code sont présents!"
else
    echo "  ✅ Aucun bloc @code trouvé"
fi

# 3. Vérifier les namespaces
echo ""
echo "3️⃣  Namespaces:"
if grep -q "namespace netIPAM.Components.Pages.Subnets" "$SUBNETS_DIR"/*.razor.cs; then
    echo "  ✅ Namespaces corrects"
else
    echo "  ❌ Namespaces incorrects"
fi

# 4. Vérifier les classes partielles
echo ""
echo "4️⃣  Classes partielles:"
COUNT=$(grep -c "public partial class" "$SUBNETS_DIR"/*.razor.cs)
if [ "$COUNT" -eq 4 ]; then
    echo "  ✅ 4 classes partielles trouvées"
else
    echo "  ❌ Attendu 4, trouvé $COUNT"
fi

echo ""
echo "✨ Vérification terminée!"
```

**Utilisation:**
```bash
chmod +x verify_refactor.sh
./verify_refactor.sh
```

---

## 📊 Comparaison Avant/Après

### Fichier Edit.razor

| Aspect | Avant | Après |
|--------|-------|-------|
| Lignes totales | 130 | 47 |
| @code | ✅ Présent | ❌ Absent |
| @inject | ✅ Présent | ❌ Absent |
| Complexité visuelle | Élevée | Réduite |
| Lisibilité | Faible | Forte |
| Testabilité | Difficile | Facile |

---

## 🎓 Points Clés à Retenir

1. **Pattern Partial Class**
   - Le fichier `.razor` instancie le composant
   - Le fichier `.razor.cs` contient le code métier
   - C# utilise `partial` pour les fusionner à la compilation

2. **Dépendances**
   - Préférer `[Inject]` dans `.cs` à `@inject` dans `.razor`
   - Plus cohérent avec les conventions C#

3. **Namespaces**
   - **Important**: le namespace doit correspondre au chemin du dossier
   - `Components/Pages/Subnets/Edit.razor` → `namespace netIPAM.Components.Pages.Subnets`

4. **Classe et Fichier**
   - Le nom de la classe partielle doit correspondre au nom du fichier `.razor`
   - `Edit.razor` → `public partial class Edit`

---

## 📚 Ressources

- [Microsoft Docs - Razor Components](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- Pattern utilisé dans: `Components/Pages/Admin/`

---

## ❓ Questions Fréquentes

**Q: Dois-je supprimer les fichiers originaux?**  
R: Oui, après vérification que tout fonctionne. Gitez d'abord les anciens fichiers.

**Q: Puis-je laisser le code dans le .razor?**  
R: Techniquement oui, mais ce n'est pas l'approche recommandée pour la maintenabilité.

**Q: Comment tester les changements?**  
R: Lancer `dotnet run` et naviguer dans l'application.

**Q: Dois-je modifier les dépendances du projet?**  
R: Non, il n'y a pas de nouvelles dépendances.

---

**Dernière mise à jour:** 3 mai 2026  
**Version:** 1.0  
**Statut:** Prêt à l'emploi

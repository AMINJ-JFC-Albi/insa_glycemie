# INSA Glycemie

Document de référence à lire pour l'utilisation du dépôt git et le
développement. Merci de vous conformer aux bonnes pratiques énumérées
dans ce document.

## Equipe

- Valentin PRADEL
- Florian LLORET
- Alaric CHARPENTIER
- Franck GARCIA GONZALEZ
- Vincent RODRIGUEZ

Etudiants en M1 AMINJ, INU Champollion, année universitaire 2023-2024

# Pratiques de développement

## Git flow

Le dépôt contient de base deux branches :
- main
- dev

Main héberge les versions stables du jeu, dev les versions
intermédiaires lors du développement.

Les membres de l'équipe doivent travailler sur des branches qui leur
sont propres, par feature. Le nom de la branche doit contenir le
tetragramme du développeur, suivi d'un tiret bas _ puis le nom de la
feature en camel case.

ex : VROD_controlsVR

Les branches individuelles doivent toutes hériter de dev. C'est à la
discrétion du développeur de se mettre à jour le plus régulièrement
possible pour minimiser le risque de conflit.

Il est interdit de push sur main lors du développement.
Lorsqu'une feature est prête, Le développeur met à jour son
dev local, puis effectue un rebase sur sa branche. Après
résolution des potentiels conflits sur sa branche, il peut
alors effectuer un merge.

Une fois le merge terminé, la branche peut être supprimée en local,
mais restera préservée sur le dépôt distant. En cas de problème 
(régression), perte de travail ou conflit mal géré, l'auteur de 
l'erreur, identifié par git-blame, devra apporter des chocolatines au
reste de l'équipe. Si l'auteur de l'erreur parvient à résoudre le 
problème avant la fin de la journée, sa sentence est levée.

Les notes de version, mises à jour régulièrement, suivront l'architecture suivante:
> ## {type:`a`;`b`;`pre` avant une version `stable`}{version-majeure}.{version-mineure}.{version-revision}
> - Résumé de la version : une brève description des principales modifications ou ajouts de cette version.
> 
> ### Nouvelles features
> 
> - Liste des nouvelles fonctionnalités implémentées dans cette version.
> 
> ### Modifications apportées
> 
> Détails des modifications apportées à des fonctionnalités existantes ou des ajustements mineurs. 
> 
> ### Problèmes connus
> 
> - Liste des bugs ou des problèmes connus qui n'ont pas encore été corrigés, avec des détails sur les conditions dans lesquelles ils se produisent.

# Code

## Version d'unity choisie

6000.0.23f1

## Langages utilisés

- C# via le framework Unity.

## Pratiques

Le code source sera séparé en classes et organisé selon des namespaces 
suivant une arborescence structurée. Exception faite des types énumérés, aucun
fichier source ne contiendra plus de une seule classe.

Dans la mesure du possible, le code sera testé au
maximum. On évitera au maximum de commit du code non testé.

Les projets et solutions de tests ne seront pas déployés lors de la
génération des exécutables en release. Ils resteront toutefois
présent sur le git pour attester du bon fonctionnement du code et
permettre de tester la non-régression.

# Notes de version

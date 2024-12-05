# Déploiement des ressources Azure pour le projet LinkUp

Ce document décrit le processus de création des ressources Azure nécessaires pour héberger une base de données Cosmos DB et une Web App, en utilisant Azure CLI.

## Contributeurs 

- Cyprien TOURAINE
- Marwan HAMDAOUI
- Léo BOUFFARD

## Lancer en local

```bash
dotnet watch run
```

Afin de pouvoir exécuter le projet en local.

## Prérequis

Avant de commencer, vous devez disposer de :

- Un compte Azure avec des privilèges suffisants pour créer des ressources.
- Azure CLI installé sur votre machine, ou vous pouvez utiliser [Azure Cloud Shell](https://shell.azure.com/) qui ne nécessite aucune installation locale.

## Étapes de création des ressources Azure

### 1. Créer un groupe de ressources

La première étape consiste à créer un groupe de ressources qui contiendra toutes les ressources suivantes (Cosmos DB, Web App, etc.).

```bash
az group create --name linkup-rg --location francecentral
```
Cette commande crée un groupe de ressources nommé linkup-rg dans la région France Central.

2. Créer une base de données Cosmos DB en mode serverless
a. Créer le compte Cosmos DB
Nous allons créer un compte Cosmos DB en mode serverless dans la région France Central.

```bash
az cosmosdb create \
  --name linkup-cosmos \
  --resource-group linkup-rg \
  --locations regionName=francecentral \
  --enable-serverless
```
b. Créer la base de données link-up-bdd
Ensuite, nous créons une base de données nommée link-up-bdd dans le compte Cosmos DB créé.

```bash
az cosmosdb sql database create \
  --account-name linkup-cosmos \
  --resource-group linkup-rg \
  --name link-up-bdd
```
c. Créer les conteneurs dans la base de données
Nous allons maintenant créer trois conteneurs dans la base de données link-up-bdd, chacun avec sa propre clé de partition.

Conteneur user avec clé de partition /user_id :
```bash
az cosmosdb sql container create \
  --account-name linkup-cosmos \
  --resource-group linkup-rg \
  --database-name link-up-bdd \
  --name user \
  --partition-key-path "/user_id"
```
Conteneur media avec clé de partition /media_id :
```bash
az cosmosdb sql container create \
  --account-name linkup-cosmos \
  --resource-group linkup-rg \
  --database-name link-up-bdd \
  --name media \
  --partition-key-path "/media_id"
```
Conteneur content avec clé de partition /content_id :
```bash
az cosmosdb sql container create \
  --account-name linkup-cosmos \
  --resource-group linkup-rg \
  --database-name link-up-bdd \
  --name content \
  --partition-key-path "/content_id"
```
1. Créer une Web App en .NET 8 LTS sur Windows
a. Créer un plan App Service
Avant de créer la Web App, nous devons créer un plan App Service, qui définit les ressources et les capacités de l'application Web. Nous allons utiliser un plan Windows pour la compatibilité avec .NET 8 LTS.

```bash
az appservice plan create \
  --name linkup-plan \
  --resource-group linkup-rg \
  --location francecentral \
  --sku B1 \
  --is-linux false
```
b. Créer la Web App
Enfin, nous créons une Web App sur Windows avec le runtime .NET 8 LTS.

```bash
az webapp create \
  --name linkup-webapp \
  --resource-group linkup-rg \
  --plan linkup-plan \
  --runtime "DOTNET|8-lts"
```
1. Vérification des ressources créées
Après avoir créé toutes les ressources, vous pouvez vérifier qu'elles ont bien été créées en utilisant la commande suivante :

```bash
az resource list --resource-group linkup-rg --output table
```
Cette commande affichera toutes les ressources présentes dans le groupe linkup-rg.
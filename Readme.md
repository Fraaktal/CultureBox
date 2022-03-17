
![Workflow](https://github.com/Fraaktal/CultureBox/actions/workflows/workflow.yml/badge.svg)
![](https://img.shields.io/github/languages/top/Fraaktal/CultureBox)
![](https://img.shields.io/github/issues-raw/Fraaktal/CultureBox)
![](https://img.shields.io/github/issues-closed-raw/Fraaktal/CultureBox)
[![Documentation Status](https://readthedocs.org/projects/ansicolortags/badge/?version=latest)](http://ansicolortags.readthedocs.io/?badge=latest)
[![GitHub license](https://img.shields.io/github/license/Fraaktal/CultureBox.svg)](https://github.com/Fraaktal/CultureBox/blob/master/LICENSE)
[![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](http://perso.crans.org/besson/LICENSE.html)
[![GitHub tag](https://img.shields.io/github/tag/Fraaktal/CultureBox.svg)](https://GitHub.com/Fraaktal/CultureBox/tags/)
[![GitHub latest commit](https://badgen.net/github/last-commit/Fraaktal/CultureBox)](https://GitHub.com/Fraaktal/CultureBox/commit/)
[![Docker](https://badgen.net/badge/icon/docker?icon=docker&label)](https://https://docker.com/)
![](https://img.shields.io/github/repo-size/Fraaktal/CultureBox)

## Contributors
![Contributors](https://contrib.rocks/image?repo=Fraaktal/CultureBox) 

# CultureBox

## Description (a retravailler)
Cette application est destinée a répertorier et gérer les collections de livres des utilisateurs. Chaque utilisateur peut créer plusieurs collections auxquelles, il peut ajouter et retirer des livres. Ces derniers auront un certain nombre d'informations renseignées comme leur titre, leurs auteurs ou encore les thèmes qu'ils abordent.
Notre application se sert d'une Api Google et est contenue dans/sur Docker.
Etant hebergé sur un serveur, la page web est constamment accessible.



#### Documentation des Routes
[Swagger](http://server-fraaktal.ddns.net:4208/swagger/index.html#/User/User_GetAllUser)


#### Docker
[Docker](https://hub.docker.com/r/fraaktal/culturebox)  

` docker pull fraaktal/culturebox:latest `


## Librairies (A verifier)
-Microsoft AspNetCore

-Microsoft Extensions

-Linq

-LiteDB

-Google.Apis

## Documentation des Fonctions (Completer Utilisateur -> GetApiKey)

#### Gestion des Livres

GetAll -> permet de récupérer tous les livres

GetBookById -> permet de récupérer un livre selon un identifiant 

SearchBook -> permet de rechercher un livre à partir des inforamtions que l'on dispose (catégorie, titre, etc)

#### Gestion des Utilisateurs

Get -> permet de récupérer un livre selon un identifiant

GetAllUser -> permet de récupérer tous les utilisateurs

GetApiKey -> permet de te connecter sans mote de passe et nom d'utilisateur 

CreateUser -> permet de créer un utilisateur avec un mot de passe et un pseudo unique

DeleteUser -> permet de supprimer un utilisateur

#### Gestion de Collections de livres

GetAllCollection -> permet de récupérer toutes les collections d'un utilisateur

GetCollectionById -> permet de récupérer une collection selon un identifiant

CreateCollection -> permet de créer une collection

DeleteCollection -> permet de supprimer une collection

AddBookToCollection -> permet d'ajouter un livre dans une collection

RemoveBookFromCollection -> permet de retirer un livre d'une collection


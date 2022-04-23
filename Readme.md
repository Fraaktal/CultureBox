![Workflow](https://github.com/Fraaktal/CultureBox/actions/workflows/workflow.yml/badge.svg)
![](https://img.shields.io/github/languages/top/Fraaktal/CultureBox)
![](https://img.shields.io/github/issues-raw/Fraaktal/CultureBox)
![](https://img.shields.io/github/issues-closed-raw/Fraaktal/CultureBox)
[![Documentation Status](https://readthedocs.org/projects/ansicolortags/badge/?version=latest)](http://ansicolortags.readthedocs.io/?badge=latest)
[![GitHub license](https://img.shields.io/github/license/Fraaktal/CultureBox.svg)](https://github.com/Fraaktal/CultureBox/blob/master/LICENSE)
[![GPLv3 license](https://img.shields.io/badge/License-GPLv3-blue.svg)](http://perso.crans.org/besson/LICENSE.html)
[![GitHub tag](https://img.shields.io/github/tag/Fraaktal/CultureBox.svg)](https://GitHub.com/Fraaktal/CultureBox/tags/)
[![GitHub latest commit](https://badgen.net/github/last-commit/Fraaktal/CultureBox)](https://GitHub.com/Fraaktal/CultureBox/commit/)
[![Docker](https://badgen.net/badge/icon/docker?icon=docker&label)](https://hub.docker.com/r/fraaktal/culturebox)
![](https://img.shields.io/github/repo-size/Fraaktal/CultureBox)

## Contributors
![Contributors](https://contrib.rocks/image?repo=Fraaktal/CultureBox) 

# CultureBox

## Description
Cette application est destinée a répertorier et gérer les collections de livres, films et séries des utilisateurs. Chaque utilisateur peut créer plusieurs collections auxquelles, il peut ajouter et retirer des objets. Ces derniers auront un certain nombre d'informations renseignées comme leur titre par exemple.
Notre application se sert d'une Api Google et IMDB et est contenue dans un Docker.
Étant hebergé sur un serveur, la page web est constamment accessible.



#### Documentation des Routes
[Swagger](http://server-fraaktal.ddns.net:4208/swagger/index.html#)


#### Docker
[Docker](https://hub.docker.com/r/fraaktal/culturebox)  

` docker pull fraaktal/culturebox:latest `

#### Monitoring
[Prometheus](http://server-fraaktal.ddns.net:4209/targets)
[Grafana](http://server-fraaktal.ddns.net:4210/)  


## Librairies
-Microsoft AspNetCore

-LiteDB

-Google.Apis

-RestSharp

## Lancement de l'application
Afin de compiler et d'exécuter l'application, il est nécessaire d'utiliser Visual Studio 2019 ou 2022 avec les modules "développement .Net multiplateforme" (pour ASP.NET Core) et "Développement Web et ASP.NET". 
Importer la solution dans le logiciel et tout devrait se lancer automatiquement dans un docker.
S'il y a des problèmes pour exécuter l'application, vous pouvez contacter Tom R.

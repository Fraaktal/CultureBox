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
[Swagger](http://server-fraaktal.ddns.net:4208/swagger/index.html#/User/User_GetAllUser)


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

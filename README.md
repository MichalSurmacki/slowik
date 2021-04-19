# Słowik
## Cel projektu 
Celem projektu jest stworzenie aplikacji internetowej umożliwiającej tworzenie opisu profilów wyrazów w zadanych korpusach.

Każdy profil powinnien być opisany za pomocą:

a) ilości wystąpień w korpusie

b) listy nazw (numerów) dokumentów z korpusu, w których pojawił się wyraz (wraz z ilością)

c) listy kolokacji po lewej  z możliwością uwzględnienia nieprzekraczania zdania.

d) listy kolokacji po prawej z możliwością uwzględnienia nieprzekraczania zdania.

e) czy wyraz jest elementem jakiegoś wielowyrazowca, jeśli tak - jakiego

f) czy w obrębie zdania wyraz jest poprzedzany jednym z predefiniowanych markerów /jak często/ w jakim sąsiedztwie

g) WSD - jak często w jakim znaczeniu się pojawia 

h) jak często pojawia się jako podmiot lub orzeczenie

i) czy bywa elementem jakiś nazw własnych albo sytuacji albo przestrzennych (jeśli tak - lista)

## Zastosowane technologie
Aplikacja stworzona jest za pomocą platformy ASP.NET przy użyciu wzorca projektowego MVC.

W celu tworzenia profili wyrazów w zadanych korpusach wykorzystano platformę https://ws.clarin-pl.eu/.

## Instalacja

Docker
## Przykład użycia


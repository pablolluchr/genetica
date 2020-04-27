@startuml
MoveToWater : move to water
MoveToWater --> Drink : water near
MoveToWater --> Wander : threat

Drink : replenish thirst
Drink --> Wander : !thirsty | threat

MoveToFood : move to food
MoveToFood --> Eat : food near
MoveToFood --> Wander : threat

Eat : replenish hunger
Eat --> Wander : !hungry | threat

MoveToMate : move to mate
MoveToMate --> Mate : mate near
MoveToMate --> Wander : threat

Mate : make baby
Mate --> Wander : baby made | threat

MoveToFuel : move to fuel
MoveToFuel --> Harvest : fuel near
MoveToFuel --> Wander : threat

Harvest : load up with fuel
Harvest --> Wander : no biofuel | storage full | threat

MoveToBase : move to base
MoveToBase --> Wander : threat | base near

EngageEnemy : move to enemy
EngageEnemy --> Attack : enemy near
EngageEnemy --> Flee : v.hungry | v.thirsty | low health

Attack : damage enemy
Attack --> Flee : low health
Attack --> EngageEnemy: !enemy near
Attack --> Wander : !threat

Flee : move in direction of fewest enemies
Flee --> Wander : !threat

Dead : dead

[*] --> Wander
Wander : walk to destination
Wander --> EngageEnemy : threat & aggressive
Wander --> Flee : threat & !aggressive
Wander --> MoveToWater : thirsty & see water
Wander --> MoveToFood : hungry & see food
Wander --> MoveToBase : carrying fuel | need change
Wander --> MoveToMate : horny & see mate
Wander --> MoveToFuel : see fuel
@enduml
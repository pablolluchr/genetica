@startuml
MoveToWater : move to water
MoveToWater --> Wander : threat
MoveToWater --> Drink : water near

Drink : replenish thirst
Drink --> Wander : !thirsty | threat

MoveToFood : move to food
MoveToFood --> Wander : threat
MoveToFood --> Eat : food near

Eat : replenish hunger
Eat --> Wander : !hungry | threat

MoveToMate : move to mate
MoveToMate --> Wander : threat
MoveToMate --> Mate : mate near

Mate : make baby
Mate --> Wander : baby made | threat

MoveToFuel : move to fuel
MoveToFuel --> Wander : threat
MoveToFuel --> Harvest : fuel near

Harvest : load up with fuel
Harvest --> Wander : no biofuel | storage full | threat

MoveToBase : move to base
MoveToBase --> Wander : threat | base near

EngageEnemy : move to enemy
EngageEnemy --> Flee : v.hungry | v.thirsty | low health
EngageEnemy --> Attack : enemy near

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
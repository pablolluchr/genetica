@startuml
MoveToWater : move to water
MoveToWater --> Wander : threat | !see water
MoveToWater --> Drink : near

Drink : replenish thirst
Drink --> Wander : !thirsty | threat

MoveToFood : move to food
MoveToFood --> Wander : threat | !see food
MoveToFood --> Eat : near food

Eat : replenish hunger
Eat --> Wander : !hungry | threat

MoveToMate : move to mate
MoveToMate --> Wander : threat | !see mate
MoveToMate --> Mate : near nate

Mate : make baby
Mate --> Wander : !horny | threat

MoveToFuel : move to fuel
MoveToFuel --> Wander : threat | !see fuel
MoveToFuel --> Harvest : near fuel

Harvest : load up with fuel
Harvest --> Wander : no biofuel | storage full | threat

MoveToBase : move to base
MoveToBase --> Wander : threat | near base

EngageEnemy : move to enemy
EngageEnemy --> Flee : v.hungry | v.thirsty | low health
EngageEnemy --> Attack : near enemy
EngageEnemy --> Wander : !threat

Attack : damage enemy
Attack --> Flee : low health
Attack --> EngageEnemy: !near enemy
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
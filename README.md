# UltimateHUD
### Requirements
- Exiled 9.6.0 or newest
- Hint Service Meow [V5.4.0](https://github.com/MeowServer/HintServiceMeow/releases/tag/V5.4.0)
### Gameplay
![image](https://github.com/user-attachments/assets/8595f42f-7ffe-4443-bb54-b02407b8ac42)

### Spectator
![image](https://github.com/user-attachments/assets/51255713-8c8e-41f5-a474-8d84aa37b7e8)

### Config

```yaml
is_enabled: true
debug: false
ms_refresh_rate: 500
# Clock Settings:
enable_clock: true
clock: '<color={color}><b>Time:</b> {time}</color>'
# UTC Time Zone | 2 = UTC+2
time_zone: 2
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
clock_visual: 'BOTH'
clock_x_cordinate: -480
# TPS Settings:
enable_tps: true
tps: '<color={color}><b>TPS:</b> {tps}/{maxTps}</color>'
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
tps_visual: 'BOTH'
tps_x_cordinate: -60
# ROUND TIME Settings:
enable_round_time: true
round_time: '<color={color}><b>Round Time:</b> {round_time}</color>'
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
round_time_visual: 'BOTH'
round_time_x_cordinate: 400
# HUD Hints:
player_hud: '<color=#808080><b>Nick:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>'
spectator_hud: '<color=#808080><b>Spectating:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>'
spectator_map_info: '<b>Generators:</b> <color=orange>{engaged}/{maxGenerators}</color> <b>| Warhead:</b> <color={warheadColor}>{warheadStatus}</color>'
spectator_server_info: '<b>Players:</b> <color=orange>{players}/{maxPlayers}</color> <b>| Spectators:</b> <color=orange>{spectators}</color>'
```

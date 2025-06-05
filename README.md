![UltimateHUD](https://github.com/user-attachments/assets/624b8a88-7269-452a-a507-f87ca6363179)<br><br><br>
[![downloads](https://img.shields.io/github/downloads/Vretu-Dev/UltimateHUD/total?style=for-the-badge&logo=icloud&color=%233A6D8C)](https://github.com/Vretu-Dev/UltimateHUD/releases/latest)
![Latest](https://img.shields.io/github/v/release/Vretu-Dev/UltimateHUD?style=for-the-badge&label=Latest%20Release&color=%23D91656)

## Downloads:
| Framework | Version    |  Release                                                              |
|:---------:|:----------:|:----------------------------------------------------------------------:|
| Exiled    | ≥ 9.6.0    | [⬇️](https://github.com/Vretu-Dev/UltimateHUD/releases/latest)        |
| LabAPI    | 1.0.2      | [⬇️](https://github.com/Vretu-Dev/UltimateHUD/releases/latest) |

## Requirements:
- Hint Service Meow [V5.4.0](https://github.com/MeowServer/HintServiceMeow/releases/tag/V5.4.0)

## Features:
- Clock
- TPS
- RoundTime
- Player Info: Nick, ID, Role Name, Kills
- Spectator Info: Spectating nick, id, role and kills
- Players count
- Spectators count
- Engage generators count
- Warhead status

## Credits:
- Thanks [@NamelessSCP](https://github.com/NamelessSCP) for using the [SpectatorList](https://github.com/NamelessSCP/SpectatorList-SL) idea.<br>

## Config:
```yaml
is_enabled: true
debug: false
# Clock Settings:
enable_clock: true
clock: '<color={color}><b>Time:</b> {time}</color>'
# UTC Time Zone | 2 = UTC+2
time_zone: 2
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
clock_visual: 'BOTH'
clock_x_cordinate: -480
clock_y_cordinate: 20
# TPS Settings:
enable_tps: true
tps: '<color={color}><b>TPS:</b> {tps}/{maxTps}</color>'
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
tps_visual: 'BOTH'
tps_x_cordinate: -60
tps_y_cordinate: 20
# ROUND TIME Settings:
enable_round_time: true
round_time: '<color={color}><b>Round Time:</b> {round_time}</color>'
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
round_time_visual: 'BOTH'
round_time_x_cordinate: 400
round_time_y_cordinate: 20
# Player HUD Settings:
enable_player_hud: true
player_hud: '<color=#808080><b>Nick:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>'
# Spectator List:
enable_spectator_list: true
spectator_list_header: "<color={color}>\U0001F465 Spectators ({count})</color>"
spectator_list_players: '<color={color}>• {nickname}</color>'
hidden_for_roles:
- Overwatch
spectator_list_y_cordinate: 100
# Spectator HUD Settings:
enable_spectator_hud: true
spectator_hud: '<color=#808080><b>Spectating:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color>'
# Spectator Map Info:
enable_spectator_map_info: true
spectator_map_info: '<b>Generators:</b> <color=orange>{engaged}/{maxGenerators}</color> <b>| Warhead:</b> <color={warheadColor}>{warheadStatus}</color>'
map_info_x_cordinate: 650
map_info_y_cordinate: 1000
# Spectator Server Info:
enable_spectator_server_info: true
spectator_server_info: '<b>Players:</b> <color=orange>{players}/{maxPlayers}</color> <b>| Spectators:</b> <color=orange>{spectators}</color>'
server_info_x_cordinate: -500
server_info_y_cordinate: 1000
```

## Showcase:
### Gameplay
![image](https://github.com/user-attachments/assets/8595f42f-7ffe-4443-bb54-b02407b8ac42)

### Spectator
![image](https://github.com/user-attachments/assets/51255713-8c8e-41f5-a474-8d84aa37b7e8)

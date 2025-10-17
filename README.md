![UltimateHUD](https://github.com/user-attachments/assets/624b8a88-7269-452a-a507-f87ca6363179)<br><br><br>
[![downloads](https://img.shields.io/github/downloads/Vretu-Dev/UltimateHUD/total?style=for-the-badge&logo=icloud&color=%233A6D8C)](https://github.com/Vretu-Dev/UltimateHUD/releases/latest)
![Latest](https://img.shields.io/github/v/release/Vretu-Dev/UltimateHUD?style=for-the-badge&label=Latest%20Release&color=%23D91656)

## Downloads:
| Framework | Version    |  Release                                                              |
|:---------:|:----------:|:----------------------------------------------------------------------:|
| Exiled    | ≥ 9.9.X    | [⬇️](https://github.com/Vretu-Dev/UltimateHUD/releases/latest)        |
| LabAPI    | 1.0.2      | [⬇️](https://github.com/Vretu-Dev/UltimateHUD/releases/latest) |

## Requirements:
- #### RueI [V3.1.X](https://github.com/pawslee/RueI/releases)
- #### 0Harmony.dll 2.2.2
> [!IMPORTANT]
> Place the RueI.dll file in the LabAPI plugin folder. <br>
Place the 0Harmony.dll file in the LabAPI dependencies folder.

> [!CAUTION]
> Plugins using RueI V2 will not work.
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
<details>
<summary><b>7777.yml</b></summary><br>

```yaml
is_enabled: true
debug: false
# Clock Settings:
enable_clock: true
clock: '<space=-480><color={color}><size=25><b>Time:</b> {time}</size></color>'
# UTC Time Zone | 2 = UTC+2
time_zone: 2
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
clock_visual: 'BOTH'
clock_y_cordinate: 980
# TPS Settings:
enable_tps: true
tps: '<space=-60><color={color}><size=25><b>TPS:</b> {tps}/{maxTps}</size></color>'
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
tps_visual: 'BOTH'
tps_y_cordinate: 980
# ROUND TIME Settings:
enable_round_time: true
round_time: '<space=400><color={color}><size=25><b>Round Time:</b> {round_time}</size></color>'
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
round_time_visual: 'BOTH'
round_time_y_cordinate: 980
# Player HUD Settings:
enable_player_hud: true
# You can use {displayname} instead of {nickname}
player_hud: '<size=33><color=#808080><b>Nick:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color></size>'
player_hud_y_cordinate: 15
# Spectator List:
enable_spectator_list: true
spectator_list_header: "<align=right><size=28><color={color}>\U0001F465 Spectators ({count})</color></size></align>"
spectator_list_players: '<align=right><size=28><color={color}>• {nickname}</color></size></align>'
hidden_for_roles:
- Overwatch
spectator_list_y_cordinate: 800
# Ammo Counter:
enable_ammo_counter: true
weapon_name: '<size=28><space=-900><color={color}>{weapon}</color> <alpha=#00>tttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt</size>'
ammo_counter: '<size=28><space=-900><b><color={color}>{current} / {max}</color></b> <alpha=#00>tttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt</size>'
ammo_counter_y_cordinate: 150
# Spectator HUD Settings:
enable_spectator_hud: true
# You can use {displayname} instead of {nickname}
spectator_hud: '<size=33><color=#808080><b>Spectating:</b> <color=white>{nickname}</color> <b>|</b> <b>ID:</b> <color=white>{id}</color> <b>|</b> <b>Role:</b> {role} <b>| Kills:</b> <color=yellow>{kills}</color></color></size>'
spectator_hud_y_cordinate: 15
# If true, will not show the spectated player's nickname when they are a skeleton (SCP-3114). This mimics the behavior of the base-game spectator UI.
hide_skeleton_nickname: true
# Spectator Map Info:
enable_spectator_map_info: true
spectator_map_info: '<space=650><size=27><b>Generators:</b> <color=orange>{engaged}/{maxGenerators}</color> <b>| Warhead:</b> <color={warheadColor}>{warheadStatus}</color></size>'
map_info_y_cordinate: 70
# Spectator Server Info:
enable_spectator_server_info: true
spectator_server_info: '<space=-500><size=27><b>Players:</b> <color=orange>{players}/{maxPlayers}</color> <b>| Spectators:</b> <color=orange>{spectators}</color></size>'
server_info_y_cordinate: 70
```

</details>

## Showcase:
### Gameplay
![image](https://github.com/user-attachments/assets/8595f42f-7ffe-4443-bb54-b02407b8ac42)

### Spectator
![image](https://github.com/user-attachments/assets/51255713-8c8e-41f5-a474-8d84aa37b7e8)

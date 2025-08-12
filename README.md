# EdgeRunners - Unity 2D Action Game

## Overview
EdgeRunners is a 2D action-platformer created in Unity. It features dynamic weapon systems, detailed armor mechanics, and RPG-style progression. Players navigate through challenging levels while collecting and equipping various weapons and armor pieces to improve their combat effectiveness.

## Core Gameplay Features

### **Movement System**
- **Horizontal Movement**: Use A/D or Arrow Keys to move left and right.
- **Jumping**: Press SPACE to jump. There is a 1.5 second cooldown between jumps.
- **Crouching**: Hold S or Down Arrow to crouch, which reduces movement speed and hitbox size.
- **Speed Mechanics**: 
  - Default speed: 5 units/sec
  - Crouching speed: 2 units/sec
  - Jump force: 7 units

### **Combat System**
- **Shooting**: Hold Left Mouse Button to fire the equipped weapon.
- **Weapon Switching**: Press 1 or 2 to switch between equipped weapons.
- **Weapon Switch Cooldown**: There is a 2-second delay when switching weapons. No gun is visible during the switch.
- **Combat Restrictions**: Players cannot shoot while crouching.

### **Dual Weapon System**
Players can equip and carry **two weapons at the same time**:
- **Slot 1**: Primary weapon slot
- **Slot 2**: Secondary weapon slot
- **Dynamic Switching**: Seamlessly switch between weapons during combat.
- **Weapon Persistence**: Equipped weapons are saved between game sessions.

## Equipment & Progression

### **Weapon Types**
Each weapon has unique stats:
- **Damage**: Bullet damage output
- **Speed**: Projectile velocity
- **Range**: Maximum bullet travel distance
- **Cooldown**: Time between shots
- **Size Multiplier**: Visual scaling of the weapon

### **Armor System**
This game has a detailed 4-piece armor system with subtypes:

**Armor Pieces:**
- **Helmet**: Protects the head
- **Chestplate**: Protects the torso  
- **Leggings**: Protects the legs
- **Boots**: Protects the feet

**Armor Benefits:**
- **HP Increase**: Flat health bonus for each piece
- **Damage Reduction**: Percentage-based damage mitigation
- **Stacking Effects**: All equipped armor pieces combine their bonuses
- **Real-time Updates**: Armor changes immediately affect player stats

### **Progression & Unlocks**
- **Level-based Unlocks**: Items require specific levels to access.
- **Persistent Progress**: Player progress and equipment automatically save.
- **Item Variety**: A large database of weapons and armor pieces is available.

## Health & Survival

### **Health System**
- **Base HP**: 100 health points before armor bonuses
- **Dynamic Max HP**: Increases with equipped armor HP bonuses
- **Damage Reduction**: Armor reduces incoming damage by percentages
- **Visual HP Bar**: Real-time health display that fills based on percentage

### **Health Regeneration**
- **Regeneration Trigger**: Health regenerates after 10 seconds without taking damage.
- **Heal Rate**: 5 HP per second during regeneration
- **Continuous Healing**: Regeneration continues until max HP is reached.

## User Interface

### **Inventory System**
- **Equipment Selection**: Click items to see detailed stats.
- **Slot Assignment**: Use "Slot 1" and "Slot 2" buttons to equip weapons to specific slots.
- **Armor Equipping**: Automatic slot assignment based on armor subtype.
- **Visual Feedback**: Equipped items show in dedicated UI slots.

### **Equipment Display**
- **Weapon Slots**: Visual representation of equipped weapons.
- **Armor Slots**: Displays for helmet, chestplate, leggings, and boots.
- **Real-time Updates**: The equipment UI updates immediately when items are equipped.

### **Statistics Panels**
**Weapon Details:**
- Damage, Speed, Range, Cooldown values

**Armor Details:**  
- HP Increase and Damage Reduction percentages

## Technical Features

### **Save System**
- **Persistent Equipment**: All equipped items are saved between game sessions.
- **Progress Tracking**: Player level and unlocked items are maintained.
- **Cross-session Continuity**: Resume gameplay with previously equipped gear.

### **Performance Optimizations**
- **Efficient Equipment Management**: Streamlined loading and caching of items.
- **Memory Management**: Automatic cleanup of unused weapon objects.
- **Smooth Weapon Switching**: Optimized system for switching weapons.

## Future Development Plans

### **Expanded Arsenal**
- **Weapon Varieties**: 
  - Assault rifles with burst fire modes 
  - Sniper rifles with zoom mechanics 
  - Shotguns with spread patterns 
  - Energy weapons with unique effects 
  - Melee weapons for close combat

### **Advanced Armor System**
- **Armor Set Bonuses**: Complete armor sets provide unique additional buffs.
- **Legendary Armor**: Rare armor pieces that have special abilities.
- **Armor Modification**: Upgrade and customize existing armor pieces.
- **Visual Armor**: Equipped armor changes the player’s appearance.

### **Boss Encounters**
- **Multi-phase Boss Fights**: Complex encounters that require strategy and skill.
- **Unique Boss Mechanics**: Each boss has distinct attack patterns.
- **Boss-specific Rewards**: Obtain exclusive weapons and armor from defeating bosses.
- **Scaling Difficulty**: Bosses adapt to player equipment and levels.

### **Level Progression**
- **Environmental Variety**: Different biomes and atmospheres
- **Increasing Difficulty**: Higher levels feature tougher enemies and challenges.
- **Dynamic Level Generation**: Procedurally generated elements enhance replayability.
- **Interactive Environments**: Terrain can be destroyed, creating environmental hazards.

### **Enhanced RPG Elements**
- **Character Skill Trees**: Specialized abilities and passive bonuses.
- **Crafting System**: Create and upgrade equipment using collected materials.
- **Achievement System**: Unlock rewards for completing specific challenges.
- **Player Statistics**: Detailed tracking of combat and progression.

### **Currency & Rarity System**
-Future updates will introduce a multi-tiered currency economy. Players will earn standard coins from combat, rare gems from elite enemies, and legendary crystals from boss encounters. This system will include different equipment tiers based on rarity: Common, Rare, Epic, and Legendary. Higher-tier weapons and armor will need premium currencies and rare materials. Players will have to decide whether to buy immediate upgrades or save for powerful legendary gear that has unique abilities, better stats, and special visual effects. The economy will include changing prices, currency conversion methods, and rotating merchant inventories. This will create engaging long-term progression goals.
***

**Built with Unity 2D | Supports Windows Platform**

using System;
using System.Collections.Generic;
using System.Text;
using ACoZ.Helpers;
using ACoZ.Npc;

namespace ACoZ.Levels
{
    internal class LevelData
    {
        private readonly int _realLevelNumber;
        private readonly int _colsPerLevel;
        private readonly int _linesPerScreen;
        private readonly int _startingPoint;
        private readonly int _endingPoint;

        const char EMPTY_TILE = '.';
        const char GROUND_TILE = '#';
        const char START_TILE = '1';
        const char END_TILE = 'X';

        public List<string> Lines { get; private set; }
        public TimeSpan EnemySpawnTimeFrom { get; private set; }
        public TimeSpan EnemySpawnTimeTo { get; private set; }
        public Dictionary<string, EnemyParameter> EnemiesParameters { get; private set; }

        public TimeSpan TimeToCompleteLevel { get; private set; }

        public LevelData(int levelNumber)
        {
            this._realLevelNumber = levelNumber + 1; // xq empieza en 0
            this._linesPerScreen = GlobalParameters.SCREEN_HEIGHT / GlobalParameters.TILE_HEIGHT;

            int colsPerScreen = GlobalParameters.SCREEN_WIDTH / GlobalParameters.TILE_WITH;
            this._colsPerLevel = colsPerScreen * GlobalParameters.INITIAL_SCREENS + colsPerScreen * GlobalParameters.ADD_SCREEN_PER_LEVEL * levelNumber;

            // Seteamos el punto de inicio en el primer cuarto de la primera pantalla del nivel
            this._startingPoint = colsPerScreen/4;
            // Seteamos el punto de finalizacion en el ultimo cuarto de la ultima pantalla del nivel
            this._endingPoint = this._colsPerLevel - this._startingPoint;

            this.Load();
        }

        private void Load()
        {
            var lines = new List<string>(this._linesPerScreen);

            for (var i = 0; i < this._linesPerScreen - 3; ++i)
            {
                lines.Add(this.GetLevelLine(EMPTY_TILE));
            }

            lines.Add(this.GetLevelLine(EMPTY_TILE, true));
            lines.Add(this.GetLevelLine(GROUND_TILE));
            lines.Add(this.GetLevelLine(EMPTY_TILE));

            this.Lines = lines;

            this.EnemySpawnTimeFrom = TimeSpan.FromSeconds(RandomUtil.NextDouble(GlobalParameters.ENEMY_SPAWN_TIME_FROM_BASE, GlobalParameters.ENEMY_SPAWN_TIME_FROM_TOP));
            this.EnemySpawnTimeTo = TimeSpan.FromSeconds(RandomUtil.NextDouble(GlobalParameters.ENEMY_SPAWN_TIME_TO_BASE, GlobalParameters.ENEMY_SPAWN_TIME_TO_TOP));
            
            //EnemySpawnTimeFrom = TimeSpan.FromSeconds(3.0f);
            //EnemySpawnTimeTo = TimeSpan.FromSeconds(5.0f);

            this.EnemiesParameters = this.GetEnemiesParameters();
            this.TimeToCompleteLevel = this.GetMinutesToCompleteLevel();

            // Solo para testing :)
            //EnemiesParameters = GetEmptyEnemiesParameters();
            //TimeToCompleteLevel = TimeSpan.FromSeconds(10);
        }

        private TimeSpan GetMinutesToCompleteLevel()
        {
            var minutesToAdd = this._realLevelNumber/GlobalParameters.ADD_MINUTES_EVERY_X_LEVEL;

            return TimeSpan.FromMinutes(GlobalParameters.INITIAL_MINUTES_TO_COMPLETE_LEVEL +
                                     minutesToAdd*GlobalParameters.MINUTES_TO_ADD_EVERY_X_LEVEL);
        }

#if DEBUG
        private Dictionary<string, EnemyParameter> GetEmptyEnemiesParameters()
        {
            return new Dictionary<string, EnemyParameter>(0);
        }
#endif

        private Dictionary<string, EnemyParameter> GetEnemiesParameters()
        {
            var enemiesPercentString = new Dictionary<string, EnemyParameter>(GlobalParameters.MAX_ENEMIES_AVAILABLE);

            //foreach (var val in Enum.GetValues(typeof(NpcTypes)))
            foreach (var val in Util.GetValues<NpcTypes>())
            {
                enemiesPercentString.Add(val.ToString(), new EnemyParameter(0, 0));
            }

            // Evaluo si ya no puedo agregar mas tipos de enemigos xq no tengo mas tipos que agregar :)
            // La parte entera de la division del nivel y ADD_ENEMY_TYPE_EVERY_X_LEVEL
            // X ej: nivel 16 / agregar enemigos cada 5 niveles = 3.2. La parte entera es 3, + 1 = 4 
            // 4 es > a 3 MAX_ENEMIES_AVAILABLE
            var enemiesTypeToAdd = this._realLevelNumber / GlobalParameters.ADD_ENEMY_TYPE_EVERY_X_LEVEL + 1;

            if (enemiesTypeToAdd > GlobalParameters.MAX_ENEMIES_AVAILABLE)
            {
                enemiesTypeToAdd = GlobalParameters.MAX_ENEMIES_AVAILABLE;
            }

            var percent = 1.0f / enemiesTypeToAdd;

            var enemiesToAddToPool = this._realLevelNumber / GlobalParameters.ADD_ENEMY_TO_POOL_EVERY_X_LEVEL + GlobalParameters.INITIAL_ENEMY_POOL;


            int enemiesToAddToPoolPerEnemy;
            
            if (enemiesToAddToPool > GlobalParameters.MAX_ENEMIES_POOL)
            {
                enemiesToAddToPoolPerEnemy = GlobalParameters.MAX_ENEMIES_POOL;
            }
            else
            {
                enemiesToAddToPoolPerEnemy = (int)Math.Floor((double)enemiesToAddToPool / enemiesTypeToAdd);   
            }

            //var npcTypesArray = (NpcTypes[])Enum.GetValues(typeof(NpcTypes));
            var npcTypesArray = Util.GetValues<NpcTypes>();

            for (var i = 0; i < enemiesTypeToAdd; i++)
            {
                enemiesPercentString[npcTypesArray[i].ToString()].Percent = percent;
                //enemiesPercentString[npcTypesArray[i].ToString()].Pool = GlobalParameters.MAX_ENEMIES_POOL;
                enemiesPercentString[npcTypesArray[i].ToString()].PoolQuantity = enemiesToAddToPoolPerEnemy;
            }
            
            this.ValidateEnemies(enemiesPercentString);

            return enemiesPercentString;
        }

        private void ValidateEnemies(Dictionary<string, EnemyParameter> enemiesPercentString)
        {
            double validateEnemies = 0;

            foreach (var percentString in enemiesPercentString)
            {
                validateEnemies += percentString.Value.Percent;
            }

            if (Math.Abs(validateEnemies - 1) > 0.005)
                throw new Exception("La sumatoria del porcentaje de aparicion de los enemigos es distinto de 1");
        }

        private string GetLevelLine(char tile, bool includeStartEndPoints)
        {
            var lineString = new StringBuilder(this._colsPerLevel);

            for (var x = 0; x < this._colsPerLevel; ++x)
            {
                if (includeStartEndPoints)
                {
                    if (x == this._startingPoint)
                    {
                        lineString.Append(START_TILE);
                    }
                    else if (x == this._endingPoint)
                    {
                        lineString.Append(END_TILE);
                    }
                    else
                    {
                        lineString.Append(tile);    
                    }
                }
                else
                {
                    lineString.Append(tile);
                }
            }
            return lineString.ToString();
        }

        private string GetLevelLine(char tile)
        {
            return this.GetLevelLine(tile, false);
        }
    }
}

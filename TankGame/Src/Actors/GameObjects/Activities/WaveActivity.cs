using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TankGame.Src.Actors.Data;
using TankGame.Src.Actors.Pawns;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Actors.Pawns.Enemy;
using TankGame.Src.Data.Map;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal class WaveActivity : Activity
    {
        protected Queue<List<EnemySpawnData>> EnemySpawns { get; }
        protected uint CurrentWave { get; set; }

        public WaveActivity(Vector2i coords, HashSet<Enemy> enemies, Queue<List<EnemySpawnData>> enemySpawns, Region region, uint currentWave = 0, int? hp = null, string name = null, string type = null, int? pointsAdded = null, Tuple<TraversibilityData, DestructabilityData, string> gameObjectType = null) : base(coords, enemies, hp ?? 1, name ?? "Destroy all enemies", type ?? "wave", gameObjectType ?? new Tuple<TraversibilityData, DestructabilityData, string>(new TraversibilityData(1, false), new DestructabilityData(1, false, false), null), pointsAdded ?? 3000)
        {
            EnemySpawns = enemySpawns;
            Region = region;
            CurrentWave = currentWave;
            AllEnemiesCount = (uint)Enemies.Count;
            if (AllEnemiesCount == 0 && (enemySpawns == null || EnemySpawns.Count == 0))
            {
                ActivityStatus = ActivityStatus.Completed;
                ChangeToCompleted();
            }

            PointsAdded *= (int)currentWave + (enemySpawns == null ? 0 : enemySpawns.Count);
        }

        protected override string CalculateProgress()
        {
            if (Enemies.Count == 0 && ActivityStatus == ActivityStatus.Started)
            {
                if (EnemySpawns.Count == 0) ChangeStatus(ActivityStatus.Completed);
                else SpawnNextWave();
            }

            if (ActivityStatus == ActivityStatus.Completed || ActivityStatus == ActivityStatus.Failed) return "";

            return "Enemy " + (AllEnemiesCount - Enemies.Count) + " of " + AllEnemiesCount + "\n" +
                   "Wave  " + CurrentWave + " of " + (CurrentWave + (EnemySpawns != null ? EnemySpawns.Count : 0));
        }

        public override void ChangeStatus(ActivityStatus activityStatus)
        {
            base.ChangeStatus(activityStatus);
            if (ActivityStatus == ActivityStatus.Started && Enemies.Count == 0 && EnemySpawns != null && EnemySpawns.Count != 0) SpawnNextWave();
        }

        protected void SpawnNextWave()
        {
            CurrentWave++;
            (from enemySpawnData in EnemySpawns.Dequeue() select EnemyFactory.CreateEnemy(enemySpawnData, -1, CurrentRegion)).ToList().ForEach(enemy =>
            {
                Enemies.Add(enemy);
            });

            AllEnemiesCount = (uint)Enemies.Count;
            if (AllEnemiesCount == 0 && EnemySpawns.Count == 0 && ActivityStatus != ActivityStatus.Completed) ChangeStatus(ActivityStatus.Completed);
        }

        internal override XmlElement SerializeToXML(XmlDocument xmlDocument)
        {
            XmlElement activityElement = xmlDocument.CreateElement("activity");
            XmlElement typeElement = xmlDocument.CreateElement("type");
            XmlElement xElement = xmlDocument.CreateElement("x");
            XmlElement yElement = xmlDocument.CreateElement("y");
            XmlElement healthElement = xmlDocument.CreateElement("health");
            XmlElement currentWaveElement = xmlDocument.CreateElement("currentWave");
            XmlElement wavesElement = xmlDocument.CreateElement("waves");

            typeElement.InnerText = Type;
            xElement.InnerText = (Coords.X % 20).ToString();
            yElement.InnerText = (Coords.Y % 20).ToString();
            healthElement.InnerText = Health.ToString();
            currentWaveElement.InnerText = CurrentWave.ToString();

            EnemySpawns.ToList().ForEach(list =>
            {
                XmlElement waveElement = xmlDocument.CreateElement("wave");
                list.ForEach(enemydata =>
                {
                    XmlElement enemyElement = xmlDocument.CreateElement("enemy");
                    XmlElement xElement = xmlDocument.CreateElement("x");
                    XmlElement yElement = xmlDocument.CreateElement("y");
                    XmlElement typeElement = xmlDocument.CreateElement("type");
                    XmlElement aimcElement = xmlDocument.CreateElement("aimc");

                    xElement.InnerText = (enemydata.Coords.X % 20).ToString();
                    yElement.InnerText = (enemydata.Coords.Y % 20).ToString();
                    typeElement.InnerText = enemydata.Type;
                    aimcElement.InnerText = enemydata.AimcType;

                    enemyElement.AppendChild(xElement);
                    enemyElement.AppendChild(yElement);
                    enemyElement.AppendChild(typeElement);
                    enemyElement.AppendChild(aimcElement);

                    if (enemydata.PatrolRoute != null && enemydata.PatrolRoute.Any())
                    {
                        XmlElement pathElement = xmlDocument.CreateElement("path");

                        foreach (Vector2i point in enemydata.PatrolRoute)
                        {
                            XmlElement pointElement = xmlDocument.CreateElement("point");
                            XmlElement xPointElement = xmlDocument.CreateElement("x");
                            XmlElement yPointElement = xmlDocument.CreateElement("y");

                            xPointElement.InnerText = (point.X % 20).ToString();
                            yPointElement.InnerText = (point.Y % 20).ToString();

                            pointElement.AppendChild(xPointElement);
                            pointElement.AppendChild(yPointElement);

                            pathElement.AppendChild(pointElement);
                        }

                        enemyElement.AppendChild(pathElement);
                    }
                    waveElement.AppendChild(enemyElement);
                });
                wavesElement.AppendChild(waveElement);
            });

            activityElement.AppendChild(typeElement);
            activityElement.AppendChild(xElement);
            activityElement.AppendChild(yElement);
            activityElement.AppendChild(healthElement);
            activityElement.AppendChild(currentWaveElement);
            activityElement.AppendChild(wavesElement);

            return activityElement;
        }
    }
}
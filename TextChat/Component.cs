using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace TextChat
{
    public class Component : MonoBehaviour
    {
        public static Config Config => Plugin.Instance.Config;
        
        private static readonly Dictionary<Player, List<string>> Queue = new ();
        
        private static readonly List<Component> Components = new ();
        
        private TextToy _toy;

        private Player _player;

        private Transform _transform;

        private string _rawText;

        public void Awake()
        {
            _transform = transform;
            Timing.CallDelayed(Plugin.Instance.Config.MessageExpireTime, Destroy, gameObject);
        }

        private void Destroy()
        {
            Components.Remove(this);
            if (!Queue.TryGetValue(_player, out List<string> texts))
            {
                _toy.Destroy();
                return;
            }

            texts.Remove(_rawText);
            string nextMessage = Queue[_player].FirstOrDefault();
            if(nextMessage != null)
                Spawn(_player, nextMessage);
            else 
                Queue.Remove(_player);
            _toy.Destroy();
        }
        
        public void Update()
        {
            if (_toy.IsDestroyed) return;
            foreach (Player player in Player.ReadyList.Where(p => p != _player))
            {
                if (Vector3.Distance(transform.position, player.Position) > 20)
                {
                    player.SendFakeSyncVar(_toy.Base, 4, Vector3.zero);
                    continue;
                }
                player.SendFakeSyncVar(_toy.Base, 4, Vector3.one);
                FaceTowardsPlayer(player);
            }
        }

        public void FaceTowardsPlayer(Player observer)
        {
            Vector3 direction = observer.Position - _transform.position;
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(-direction);
            _transform.rotation = rotation;
            
            observer.SendFakeSyncVar(_toy.Base, 2, _transform.localRotation);
        }

        public static void TrySpawn(Player player, string text)
        {
            if (!Queue.TryGetValue(player, out List<string> texts))
            {
                Queue.Add(player, new ());
                Queue[player].Add(text);
                Spawn(player, text);
            }
            else
            {
                texts.Add(text);
            }
        }

        private static void Spawn(Player player, string text)
        {
            if (!player.IsAlive || player.IsSCP) return;
            
            TextToy toy = TextToy.Create(new (0, Plugin.Instance.Config.HeightOffset, 0), player.GameObject.transform);
            toy.TextFormat = $"<size={Plugin.Instance.Config.TextSize}em>{Plugin.Instance.Translation.Prefix}{text}</size>";
            
            Component comp = toy.GameObject.AddComponent<Component>();
            
            comp._toy = toy;
            comp._player = player;
            comp._rawText = text;
            
            Components.Add(comp);
            
            toy.Base.enabled = false;
            
            player.Connection.Send(new ObjectDestroyMessage
            {
                netId = toy.Base.netId,
            });
            
            player.SendHint(string.Format(Plugin.Instance.Translation.CurrentMessage, text), Plugin.Instance.Config.MessageExpireTime);
        }

        public static bool CanSpawn(RoleTypeId role) => role.IsAlive() && !role.IsScp();
        
        public static bool ContainsPlayer(Player player) => Queue.ContainsKey(player);
        
        public static void RemovePlayer(Player player)
        {
            Queue.Remove(player);
            if (!Components.TryGetFirst(comp => comp._player == player, out Component component)) return;
            component.Destroy();
        }
    }
}
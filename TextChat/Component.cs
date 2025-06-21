using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace TextChat
{
    public class Component : MonoBehaviour
    {
        private static Dictionary<Player, List<string>> _queue = new ();
        
        private TextToy _toy;

        private Player _player;

        private Transform _transform;

        public void Awake()
        {
            _transform = transform;
            Timing.CallDelayed(Plugin.Instance.Config.MessageExpireTime, Destroy, gameObject);
        }

        private void Destroy()
        {
            if (!_queue.ContainsKey(_player))
            {
                _toy.Destroy();
                return;
            }

            _queue[_player].Remove(_toy.TextFormat);
            string nextMessage = _queue[_player].FirstOrDefault();
            if(nextMessage != null) 
                Spawn(_player, nextMessage);
            else 
                _queue.Remove(_player);
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
            if (!_queue.ContainsKey(player))
            {
                _queue.Add(player, new ());
                _queue[player].Add(text);
                Spawn(player, text);
            }
            else
            {
                _queue[player].Add(text);
            }
        }

        private static void Spawn(Player player, string text)
        {
            TextToy toy = TextToy.Create(new (0, Plugin.Instance.Config.HeightOffset, 0), player.GameObject.transform);
            toy.TextFormat = $"<size={Plugin.Instance.Config.TextSize}em>{Plugin.Instance.Translation.Prefix}{text}</size>";
            
            Component comp = toy.GameObject.AddComponent<Component>();
            comp._toy = toy;
            comp._player = player;
            
            toy.Base.enabled = false;
            
            player.Connection.Send(new ObjectDestroyMessage
            {
                netId = toy.Base.netId,
            });
            
            player.SendHint(string.Format(Plugin.Instance.Translation.CurrentMessage, text), Plugin.Instance.Config.MessageExpireTime);
        }

        public static bool CanSpawn(RoleTypeId role) => role.IsAlive() && !role.IsScp();
        
        public static bool ContainsPlayer(Player player) => _queue.ContainsKey(player);
        
        public static void RemovePlayer(Player player) => _queue.Remove(player);
    }
}
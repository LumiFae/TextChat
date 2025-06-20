using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using UnityEngine;

namespace TextChat
{
    public class Component : MonoBehaviour
    {
        // TODO: Implement queue for this component to make sure nobody breaks this stuff
        
        private TextToy _toy;

        private Player _player;

        private Transform _transform;

        public void Awake()
        {
            _transform = transform;
            Timing.CallDelayed(Plugin.Instance.Config.MessageExpireTime, () => _toy.Destroy(), gameObject);
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

        public static void Spawn(Player player, string text)
        {
            TextToy toy = TextToy.Create(new (0, Plugin.Instance.Config.HeightOffset, 0), player.GameObject.transform);
            toy.TextFormat = text;
            
            Component comp = toy.GameObject.AddComponent<Component>();
            comp._toy = toy;
            comp._player = player;
            
            toy.Base.enabled = false;
            
            player.Connection.Send(new ObjectDestroyMessage
            {
                netId = toy.Base.netId,
            });
        }
    }
}
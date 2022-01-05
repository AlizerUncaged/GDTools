using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Dashboard
{
    public class LikeBot_Task
    {
        public static Dictionary<Database.Account, List<Task<bool>>>
            Tasks = new();

        public static bool HasTask(Database.Account acc)
        {
            return Tasks.ContainsKey(acc);
        }

        private bool _like;
        private short _max;
        private int _itemID;
        private int _specialID;
        private Database.Account _owner;
        private IEnumerable<Database.Account> _likers;
        public LikeBot_Task(Database.Account owner, bool like, short max, int itemID, int specialID)
        {
            _owner = owner; _like = like; _max = max; _itemID = itemID; _specialID = specialID;
            _likers = Database.Database.GetRandomAccounts(_max);
        }

        // comence likebot
        public async Task<int> LikeBotAll()
        {
            if (!Tasks.TryGetValue(_owner, out _))
                Tasks.Add(_owner, new List<Task<bool>>());

            int success = 0;

            for (int i = 0; i < _max; i++)
            {
                Tasks[_owner].Add(AddLike());
            }

            foreach (var task in Tasks[_owner])
            {
                if (await task) success++;
            }

            // tasks done, remove it from tasks list
            Tasks.Remove(_owner);

            return success;
        }

        // adds one like
        public async Task<bool> AddLike()
        {
            // get random accounts
            throw new NotImplementedException();
            // add like
        }
    }
}

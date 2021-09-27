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


        private bool _like;
        private short _max;
        private int _itemID;
        private int _specialID;
        private Database.Account _owner;
        public LikeBot_Task(Database.Account owner, bool like, short max, int itemID, int specialID)
        {
            _owner = owner; _like = like; _max = max; _itemID = itemID; _specialID = specialID;
            List<Task<bool>> tasksList = new();

            if (!Tasks.TryGetValue(owner, out tasksList))
                Tasks.Add(owner, tasksList);
        }

        public async Task<int> LikeBotAll()
        {
            int success = 0;

            for (int i = 0; i < _max; i++)
            {
                Tasks[_owner].Add(AddLike());
            }

            foreach (var task in Tasks[_owner])
            {
                if (await task) success++;
            }
            return success;
        }

        public async Task<bool> AddLike()
        {

        }
    }
}

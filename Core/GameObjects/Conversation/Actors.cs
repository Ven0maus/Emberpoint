using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emberpoint.Core.GameObjects.Conversation
{
    static class Actors
    {
        static readonly Actor[] s_actors = {
            new(0, "You"),
            new(1, "Misterious Voice")
        };

        public static Actor GetActor(int id)
        {
            return Array.Find(s_actors, a => a.ID == id);
        }
    }

    record Actor(int ID, string Name);
}

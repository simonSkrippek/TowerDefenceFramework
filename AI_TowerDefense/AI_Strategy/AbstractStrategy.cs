using GameFramework;
using System.Collections.Generic;
namespace AI_Strategy
{
    /*
     * base class for strategies. This class needs to be inherited from to provide 
     * a proper strategy.
     */
    public abstract class AbstractStrategy
    {
        protected Player player = null;

        protected AbstractStrategy(Player player)
        {
            this.player = player;
        }

        /*
         * called by the game play environemt. For your assignment, this method needs to 
         * deliver the decision, if and where new Towers are to be deployed.
         */
        public abstract void DeployTowers();

        /*
         * called by the game play environemt. For your assignment, this method needs to 
         * deliver the decision, if and where new Soldiers are to be deployed.
         */
        public abstract void DeploySoldiers();

        /*
         * called by the game play environment. The order in which the array is returned here is
         * the order in which soldiers will plan and perform their movement.
         */
        public abstract List<Soldier> SortedSoldierArray(List<Soldier> unsortedList);
    }
}

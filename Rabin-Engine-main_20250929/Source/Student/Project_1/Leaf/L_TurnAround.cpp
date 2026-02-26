#include <pch.h>
#include "L_TurnAround.h"
#include "Agent/BehaviorAgent.h"

void L_TurnAround::on_enter()
{
    // set animation, speed, etc

    // grab the target position from the blackboard
    const auto &bb = agent->get_blackboard();

    // If facing left
    if (agent->get_yaw() == PI)
    {
      // Face the agent upwards
      agent->set_yaw(PI / 2.0f);
    }
    else
    {
      // Reverse the agents orientation
      agent->set_yaw(agent->get_yaw() + PI);
    }

	BehaviorNode::on_leaf_enter();
  on_success();
}

#include <pch.h>
#include "L_FallForward.h"
#include "Agent/BehaviorAgent.h"
#include <cmath>

void L_FallForward::on_enter()
{
  Vec3 groundOffset(0.0f, 1.0f, 0.0f);

  const auto &bb = agent->get_blackboard();

  // Rotate the agent downwards and at a slight upwards offset
  agent->set_movement_speed(0.0f);
  // agent->set_pitch(PI / 2.0f);
  
  agent->set_position(agent->get_position() + groundOffset);

	BehaviorNode::on_leaf_enter();
  display_leaf_text();
}

void L_FallForward::on_update(float dt)
{
  float epsilon = 0.001f; // Threshold 
  float speed = 10.0f;
  float desiredPitch = PI / 2.0f;

  // Smoothly rotate the agent downwards
  agent->set_pitch(std::lerp(agent->get_pitch(), desiredPitch, dt * speed));

  // Check to stop the rotation
  if (agent->get_pitch() > (PI / 2.0f) - epsilon)
    on_success();

  display_leaf_text();
}
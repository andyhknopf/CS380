#include <pch.h>
#include "L_Fly.h"
#include "Agent/BehaviorAgent.h"
#include <cmath>
#include <math.h>

void L_Fly::on_enter()
{
  const auto &bb = agent->get_blackboard();

  // Rotate the agent downwards and at a slight upwards offset
  agent->set_movement_speed(0.0f);
 
	BehaviorNode::on_leaf_enter();
  display_leaf_text();
}

void L_Fly::on_update(float dt)
{ 


  // Print debug info
  display_leaf_text();

  // Force control variables
  const float MAX_FORCE = 1.0f;
  const float MIN_FORCE = -1.0f;
  const Vec3 MAX_FORCE_VEC(MAX_FORCE, MAX_FORCE, MAX_FORCE);
  const Vec3 MIN_FORCE_VEC(MIN_FORCE, MIN_FORCE, MIN_FORCE);

  // Speed control variables
  const float MAX_SPEED = .125f;
  const float MIN_SPEED = -.15f;
  const Vec3 MAX_SPEED_VEC(MAX_SPEED, MAX_SPEED, MAX_SPEED);
  const Vec3 MIN_SPEED_VEC(MIN_SPEED, MIN_SPEED, MIN_SPEED);
  float steeringFactor = 10.0f;
  // Separation control variables
  Vec3 separationTarget(0.0f);
  float separationFactor = .5f;

  // Alignment control variables
  Vec3 alignmentTarget(0.0f);
  float alignmentFactor = 1.f;

  // Get a list of all the other agents
  float awarenessRadius = 10.0f;
  const auto& allAgents = agents->get_all_agents();
  Vec3 cumulativePositions(0.0f);
  int boidsInRadius = 1; // There is always at least 1 ("me")

  // Bounds and basic tethering
  float maxDistToCenter = 60.0f;
  Vec3 mapCenter(50.0f, 15.0f , 50.0f);

  // Change color based on position
  // Note: Must be between 0 and 1
  colorFactor += dt;
  Vec3 mapPositionOffset(cosf(colorFactor),
                         sinf(colorFactor),
                         sinf(cosf(colorFactor)));//agent->get_position().z / 100.0f); 

  // Vec3 birdColor = Vec3();
  agent->set_color(mapPositionOffset);
  agent->set_color(Vec3(1.0f, 1.0f, 1.0f));

  // For all agents
  for (const auto& a : allAgents)
  {
    // Skip if not a bird
    if (a->get_type() != "Bird")
      continue;

    // Skip if our agent
    if (a == agent)
      continue;

    // Calculate distance between us and other agents
    const auto& agentPos = a->get_position();
    const float distance = Vec3::Distance(agent->get_position(), agentPos);

    // Skip if outisde of awareness radius
    if (distance > awarenessRadius)
      continue;

    // If within radius
    ++boidsInRadius;

    // Separation alignment cohesion
    separationTarget += agent->get_position() - agentPos;
    alignmentTarget += a->get_velocity();
    cumulativePositions += agentPos;
  }

  // Calculate cumulative properties (within flocking group)
  cumulativePositions /= boidsInRadius;
  alignmentTarget /= boidsInRadius;
  
  // Set target to collective center of mass (basic tethering included)
  float distToCenter = Vec3::Distance(agent->get_position(), mapCenter);
  // alignmentTarget += (mapCenter - agent->get_position()) * distToCenter;
  //if (distToCenter > maxDistToCenter)
    //agent->set_target(mapCenter - agent->get_position());
  //else
    agent->set_target(cumulativePositions - agent->get_position());

  // THIS IS OBSTACLE AVOIDANCE OKAY RABIN
  AvoidBounds(dt);

  //  steering_dir = target - velocity;
  Vec3 steering_dir = agent->get_target() - agent->get_velocity();

  //  steering_force = truncate(steering_dir, max_force)
  Vec3 steering_force = steering_dir * steeringFactor;
  steering_force.Clamp(MIN_FORCE_VEC, MAX_FORCE_VEC);

  //  acceleration = (steering_force / mass )
  Vec3 acceleration = steering_force;
  acceleration += separationTarget * separationFactor; // Add separation force
  acceleration += alignmentTarget * alignmentFactor;
  agent->set_acceleration(acceleration);


  //  velocity = truncate(velocity + acceleration, max_speed)
  Vec3 velocity = agent->get_velocity() + agent->get_acceleration() * dt;
  velocity.Clamp(MIN_SPEED_VEC, MAX_SPEED_VEC);
  agent->set_velocity(velocity);

  //  position = position + velocity
  agent->set_position(agent->get_position() + velocity);

  // Update bird rotation
  UpdateRotation(dt);
}

void L_Fly::AvoidBounds(float dt)
{
  // Define bounds
  float ceiling = 35.0f;
  float floor = 10.0f;
  float wallMin = 0.0f;
  float wallMax = 75.0f;
  float teleportOffset = 5.0f;
  float turnFactor = .50f;
  float mag = 1;

  // Get a copy of the agents position
  Vec3 agentAcc = agent->get_acceleration();
  Vec3 agentVel = agent->get_velocity() * mag;

  // X bounds
  if (agent->get_position().x + agentVel.x  > wallMax)
    agentAcc.x -= turnFactor * dt;
  else if (agent->get_position().x + agentVel.x < wallMin)
    agentAcc.x += turnFactor * dt;

  // Y bounds
  if (agent->get_position().y + agentVel.y < floor)
    agentAcc.y += turnFactor * dt;
  else if (agent->get_position().y + agentVel.y > ceiling)
    agentAcc.y -= turnFactor * dt;
  
  // Z bounds
  if (agent->get_position().z + agentVel.z > wallMax)
    agentAcc.z -= turnFactor * dt;
  else if (agent->get_position().z + agentVel.z < wallMin)
    agentAcc.z += turnFactor * dt;

  // Teleport the agent
  agent->set_acceleration(agentAcc);
  Vec3 mapCenter(50.0f, 15.0f, 50.0f);
  float distToCenter = Vec3::Distance(agent->get_position(), mapCenter);
  Vec3 offset (mapCenter - agent->get_position());
  agentAcc = offset * (distToCenter / 2.0f);
  agent->set_target(agent->get_target() + agentAcc);
}

void L_Fly::UpdateRotation(float dt)
{
  float rotateSpeed = 10.0f;
  // Set yaw
  float newYaw = atan2f(agent->get_velocity().z, agent->get_velocity().x);
  agent->set_yaw(std::lerp(agent->get_yaw(), newYaw, rotateSpeed * dt));

  float horizLength = agent->get_velocity().z * agent->get_velocity().z;
  horizLength += (agent->get_velocity().x * agent->get_velocity().x);
  horizLength = sqrtf(horizLength);

  // Set pitch
  float newPitch = atan2f(agent->get_velocity().y, horizLength);
  agent->set_pitch(newPitch);
}



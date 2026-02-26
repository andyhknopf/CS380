#include <pch.h>
#include "Projects/ProjectOne.h"
#include "Agent/CameraAgent.h"

// Private declarations
static Vec3 RandomGreenColor();

void ProjectOne::setup()
{
    // Create an agent (using the default "Agent::AgentModel::Man" model)

    // You can change properties here or at runtime from a behavior tree leaf node
    // Look in Agent.h for all of the setters, like these:
    // man->set_color(Vec3(1, 0, 1));
    // man->set_scaling(Vec3(7,7,7));
    // man->set_position(Vec3(100, 0, 100));

    // Create an agent with a different 3D model:
    // 1. (optional) Add a new 3D model to the framework other than the ones provided:
    //    A. Find a ".sdkmesh" model or use https://github.com/walbourn/contentexporter
    //       to convert fbx files (many end up corrupted in this process, so good luck!)
    //    B. Add a new AgentModel enum for your model in Agent.h (like the existing Man or Tree).
    // 2. Register the new model with the engine, so it associates the file path with the enum
    //    A. Here we are registering all of the extra models that already come in the package.
    Agent::add_model("Assets\\tree.sdkmesh", Agent::AgentModel::Tree);
    Agent::add_model("Assets\\car.sdkmesh", Agent::AgentModel::Car);
    Agent::add_model("Assets\\bateye.sdkmesh", Agent::AgentModel::Bird);
    Agent::add_model("Assets\\ball.sdkmesh", Agent::AgentModel::Ball);
    Agent::add_model("Assets\\hut.sdkmesh", Agent::AgentModel::Hut);
    Agent::add_model("Assets\\soldier.sdkmesh", Agent::AgentModel::Soldier);

    // Create the seargant
    auto sarge = agents->create_behavior_agent("Sarge", BehaviorTreeTypes::Sarge, Agent::AgentModel::Man);
    sarge->set_color(RandomGreenColor());
    sarge->set_yaw(PI / 2);
    sarge->set_position(Vec3(35.0f, 0.0f, 50.0f));
    sarge->set_scaling(1.0f);

    // Create the soliders
    int numRows = 5;
    int numCols = 3;
    float padding = 10.0f;
    for (int i = 0; i < numCols; ++i)
    {
      for (int j = 0; j < numRows; ++j)
      {
        auto soldier = agents->create_behavior_agent("Soldier", BehaviorTreeTypes::Soldier, Agent::AgentModel::Man);
        soldier->set_color(RandomGreenColor());
        soldier->set_scaling(1.0f);
        soldier->set_yaw(PI / 2);
        soldier->set_position(Vec3(25.0f + (j * padding), 0.0f, 65.0f + (i * padding)));
      }
    }

    int runningMenCount = 5;
    for (int i = 0; i < runningMenCount; ++i)
    {
      // Create the men
      auto runningMen = agents->create_behavior_agent("RunningMan", BehaviorTreeTypes::RunningMan, Agent::AgentModel::Man);

      // Set color data
      runningMen->set_color(RandomGreenColor());

      // Set transform data
      runningMen->set_scaling(1.0f);
      runningMen->set_yaw(PI);
      runningMen->set_position(Vec3(95.0f, 0.0f, 50.0f + (i * padding)));
    }

    const int birdCount = 250;
    for (int i = 0; i < birdCount; ++i)
    {
      auto bird = agents->create_behavior_agent("Bird", BehaviorTreeTypes::Bird, Agent::AgentModel::Bird);
      bird->set_scaling(0.35f);
      bird->set_velocity(Vec3(RNG::range(0.001f, 0.1f), RNG::range(0.001f, 0.1f), RNG::range(0.001f, 0.1f)));
      bird->set_position(Vec3(RNG::range(0.0f, 10.0f), RNG::range(0.0f, 0.0f), RNG::range(0.0f, 10.0f)));
    }


    // Map to start in
    terrain->goto_map(0);

    // You can also enable the pathing layer and set grid square colors as you see fit.
    // Works best with map 0, the completely blank map
    terrain->pathLayer.set_enabled(true);
    terrain->pathLayer.set_value(0, 0, Colors::Red);

    // Camera position can be modified from this default
    auto camera = agents->get_camera_agent();
    camera->set_position(Vec3(-62.0f, 70.0f, terrain->mapSizeInWorld * 0.5f));
    camera->set_pitch(0.610865); // 35 degrees

    // Sound control (these sound functions can be kicked off in a behavior tree node - see the example in L_PlaySound.cpp)
    // audioManager->SetVolume(0.5f);
    // audioManager->PlaySoundEffect(L"Assets\\Audio\\retro.wav");
    // Uncomment for example on playing music in the engine (must be .wav)
    // audioManager->PlayMusic(L"Assets\\Audio\\motivate.wav");
    // audioManager->PauseMusic(...);
    // audioManager->ResumeMusic(...);
    // audioManager->StopMusic(...);
   
}

static Vec3 RandomGreenColor()
{
  // Calculate random data
  float randomColorMaxOffset = .25f;
  float randomColorOffset = RNG::range(0.0f, randomColorMaxOffset);

  // Apply random but green-dominant offset
  return Vec3(randomColorOffset, 1.0f - randomColorOffset, randomColorOffset);
}
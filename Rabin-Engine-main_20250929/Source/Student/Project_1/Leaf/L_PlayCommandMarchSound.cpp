#include <pch.h>
#include "L_PlayCommandMarchSound.h"

void L_PlayCommandMarchSound::on_enter()
{
	audioManager->PlaySoundEffect(L"Assets\\Audio\\command_march.wav");
	BehaviorNode::on_leaf_enter();
	on_success();
}
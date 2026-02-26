#include <pch.h>
#include "L_PlayMarchSound.h"

void L_PlayMarchSound::on_enter()
{
	audioManager->PlaySoundEffect(L"Assets\\Audio\\march.wav");
	BehaviorNode::on_leaf_enter();
	on_success();
}
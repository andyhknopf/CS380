#include <pch.h>
#include "L_PlayCommandPushUpSound.h"

void L_PlayCommandPushUpSound::on_enter()
{
	audioManager->PlaySoundEffect(L"Assets\\Audio\\command_push_up.wav");
	BehaviorNode::on_leaf_enter();
	on_success();
}
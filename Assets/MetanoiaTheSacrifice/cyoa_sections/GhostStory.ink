->Main

==Main==
#Music:WhisperLow
#BGS:Snow
A

Cloaks billowing in the wind, the two Firryn trudged onward through the feet-high snow. 

Weaving between the Wayflags and passing the aging ancient towers, they had made great progress to Winterhold.  If their luck held they'd arrive in a matter of minutes.

Most of the journey had been in silence as Iyu skillfully guided them through the mostly featureless wastes and blinding snow. Wu followed close behind, trying to memorize Iyu's movements and judgment.

It was when they had just passed the last of the Wayflags that Wu trudged up to Iyu's side. Gathering his courage, Wu looked up through frosted goggles at Iyu and tugged at his snow-covered cloak.

Iyu mentally groaned, knowing already the questions the boy had. 

Snowpilgrims didn't often meet with Snowcloaks unless there were emergencies and from the star-struck look the boy had greeted him with, he could tell Wu was fascinated with him.

* [Ignore Wu] -> no_talk
* [Humor Wu] -> talk

==talk==
VAR LivingWithWuVar = false
VAR SnowCloakProfessionVar = false

-(ConvoTop)
Wu asked Iyu about...

+ {LivingWithWuVar == false} [Living with him] ->LivingWithWu
+ {SnowCloakProfessionVar == false}[What Iyu does] -> SnowCloakProfession
+ {LivingWithWuVar == true && SnowCloakProfession == true} [That strange sound] -> Howling

-(LivingWithWu)
"Mr. Iyu, why do you live all the way up there on that cliff?"

"You could live anywhere, right?"

"We'd welcome you, you know, in the village.  My mom's always talking about how lonely you must be."

Iyu grunted. "That's nice."

"But why not? You've been up there thirty years now, right?"

"So you don't go home to Mt. Cylberyxx - you could have a home in the village!"

* [Let Wu down easy] -> SoftTurndown
* [Let Wu down hard] -> HarshTurndown

-(HarshTurndown)
"They'd starve."

"What?"

"My family," Iyu looked down at Wu, "That's the trade. I stay up there and my family gets shelter from this damn cold and some of the food you pilgrims grow."

"But Winterhold's only a few miles away!" Wu's face scrunched with doubt.

"And Springhold's another hundred, kid." Iyu sighed, grasping the back of his neck to work out the knot that was forming there. "Where your mother and the rest of the pilgrims go, I can't follow."

"Oh." Wu looked down, disappointed. 
~LivingWithWuVar = true

-(SoftTurndown)
"I'll think about it."

"Really?" Wu grabbed his cloak, hopping in excitement.

Iyu couldn't help but chuckle under his breath. "I just said I'd think about it, kid."

Wu either didn't hear him or ignored him as he excitedly chattered about the village and how well Wu would fit in there and how great the village was and how amazing the people of Winterhold were.

Iyu let the kid babble, turning his attention back to the snow around them with the faintest hint of a smile on his lips.

The smile faded away as he reminded himself that he couldn't give in to Wu's request. How could he desert his family to a life without food or shelter? To the life he was living right now?

An urgent tug on his cloak pulled him from his thoughts.  "Mr. Iyu?"

"What is it now, kid?"

~LivingWithWuVar = true

->ConvoTop

-(SnowCloakProfession)
"What do you do up there?"

"I watch for intruders. That's what Snowcloaks do - we keep you Snowpilgrims safe."
~SnowCloakProfessionVar = true
->ConvoTop
==no_talk==

Noticing Iyu's hesitation, Wu slowed his pace and fell back in line. Wu glanced back to check on him and found Wu staring at the snow, disappointment spread across his face.

'It's for the best,' Iyu thought.  There was no need to burden Wu with the harsh reality of living in Fyra at his young age.

His questions would've only ended in his disappointment.  Better to let him take the bitter pill now than build his expectations.

Iyu turned his gaze to the snow. It wouldn't be long now until they reached Winterhold.

->Howling

==Howling==

"What?" Iyu stopped suddenly as he heard it.
#Music:WhisperHigh

Something was wrong: the winds had the tones of a voice in them.  

'Spirits?' Iyu bent down and grabbed a handful of snow.  He rubbed it between his fingers but didn't see any smoke. 'No, not spirits...'

'Celanos' Madness?' He ungloved his hand, checking for the telltale blue tints, but all seemed well.

Wu, meanwhile, shuffled his feet anxiously.  His head whipped about, trying to follow the voice in the snowy winds.

A bone-chilling shriek echoed through the empty plains, stopping Iyu in his tracks. Wu shrieked as well, eyes wide with fear. "M-Mr. Iyu!"

Iyu grit his teeth. 'Banshee.' Long-limbed man-hunters who ate men to their marrow and left survivors under their curse.

Another call, this one closer than the first. Something between a shriek and wolf's howl. Moments passed - everything had become very silent and still.

Iyu looked to Winterhold -- it wasn't far now. If they made a break for it, they'd make it. If the Banshee had already caught their scent they wouldn't shake it or be able to hide from it.

Iyu's legs and heart were good for it but Wu was frozen in fear, fingers trembling and lips pressed so tight they trickled blood. He wouldn't make it.

"W-what is that, Mr. Iyu?"

* [Pick Wu up and run] ->Running

==Running==
In a burst of speed Iyu grabbed the child and hurled himself towards Winterhold. His every step smashed deep into the snow, hurling debris in all directions as the wind bit at his eyes.

The shrieking was all around now.  Iyu cursed the Gods as he realized it wasn't just one Banshee pursuing them - it was a pack.

The aged wooden walls were close now - seconds away. A confused guard stood at the gate, his hand reaching for a sword belted to his waist as Iyu charged towards him.

"Banshee," Iyu yelled, "blow your horn!"

The man shook his head, confused, and Iyu was now nearly upon him.

VAR guard_state = "fine"

* [Explain] -> Explain
* [Grab the horn and blow it] ->GrabHorn

-(Explain)
Dropping Wu to his feet, Iyu yelled at the guard as he drew his sword. "Get your horn and blow it, you fool!"

"Who are you? A Snowcloak?"

Iyu growled, "Don't you hear them, you deaf idiot? Keep rattling that sword and we'll all be dead!"

The guard finally heard one of the barks that were now rapidly approaching. "Oh, Gods! Those are Banshee howls! Kagosho protect us!"

"The horn!!"

Shaken loose from his stupor, the guard looked to Iyu and then to the horn on his belt. "The horn?" He paused for a moment and then it clicked.

"The horn," he yelled, panicked, as he ripped the thing from the string binding it to his waist and brought it to his lips. ->HornBlow

-(GrabHorn)
~guard_state = "wounded"
Dropping Wu quickly, Iyu hurled his shoulder into the man's stomach. Wheezing, doubled-over, the man staggered backwards. In that moment of weakness, Iyu ripped the nicely tied horn from the man's waist and brought it to his lips.

The yapping, barking, howls of the Banshee was all around. In moments, they would pounce. With a hurried breath, Iyu brought the horn to his lips and blew as hard as he could.
->HornBlow

-(HornBlow)
A squeeling, screetching, nails-on-chalkboard-like sound rushed out and the Banshee shrieked along with it.  {guard_state == "wounded": Iyu's eyes closed tightly as he pushed past the pain of the sound and continued to blow.}

Wu and the guard both groaned, clasping their ears as their bodies were wracked with pain. Whatever they all felt, the Banshees were feeling ten-fold, their senses sharp enough to track men through heavy snow and fog from miles away.

The yipping and barking turned to pained whines and screams. The sounds tore into the distance as the Banshee fled. 

{guard_state == "wounded": Iyu continued blowing, just for good measure, for several seconds after he could no longer hear them.}

#Music:Surreal
{guard_state == "wounded": -> GuardWounded}
{guard_state == "fine": -> GuardFine}

==GuardWounded==
He turned to check on Wu but was knocked to his feet by an iron-clad fist. Vision blurred and head pounding, he looked up to stare down the loaded channel of an ornate looking crossbow.

At the other end of it was a tall Firryn woman with wild brown hair and eyes lit in rage. Springing forward, Wu grabbed at the crossbow and struggled to pull it out of her hands.

"No!  Don't shoot Mr. Iyu!"

The woman's eyes widened as, in the struggle, the bolt loosed.

The world went white in a flash.

The next thing Iyu felt was a furred hand shaking him. Groggily, his eyes opened. Wu and the woman both leaned over his prone body. 

The snow around his head had been blown away and just inches from his ear rested a thick, black, bolt sunk several inches into the rocky ground.

Shaking off the drowsiness, Iyu came to his feet. The woman helped him and patted him on the back.

"You alright? You must have the blessing of Voshim - that bolt near took your head off thanks to your kid here."

Wu looked away, cheeks rosy and eyes red from tears.

Iyu grunted. "I doubt I've the blessing of any of the Gods. Else, I'd be home by now."

->GuardFine.IzaldaBridge

==GuardFine==
Stomping up, a huge Firryn woman with a heavy crossbow slung across her back yelled at the guard. "What in Zohulus' False Fires was that, Kurrin?"

She and the guard immediately launched into debate over when to use the horn and what he thought he heard versus what Iyu told him. Iyu had half a mind to butt in - but Wu's shaking body caught his eye.

Getting down on one knee, Iyu comforted the boy with a hand on his shoulder. "You alright, kid?"

Cheeks wet with tears, he forced a smile and nodded. "Y-yeah, Mr. Iyu! I'm fine."

* [It's alright to be afraid.] ->AlrightFear
* [You're home now.] ->HomeNow

-(AlrightFear)
Wu looked down at the dirty snow. He opened his mouth but then closed it. He furrowed his brow. "You weren't afraid."

Iyu laughed. "Yeah, kid, I was. Anyone who can face down a Banshee with no fear isn't Firryn."

"You're... different." Wu looked up at Iyu, something deep behind his eyes. His expression was as complicated as his emotions. "Everyone says the Snowcloaks are fearless and dangerous."

"I... I didn't think you were dangerous.  But you're alone up there, all the time, right?"

Wu's eyes sunk to the ground, fighting back tears. "I'm alone a lot too - and I get afraid when I'm alone. And sad. I thought you'd..."

Wiping his eyes with the back of his hand, Wu stood on shaky feet.  Iyu helped steady him. He was thinking up a reply when the huge Firryn clapped his shoulder.

"You and I, Snowcloak," she growled as she spun him around to face her, her eyes darting to the sniffling Wu and back to Iyu, "we're gonna have a talk about responsibility."

->IzaldaBridge

-(HomeNow)
"Take it easy, kid. You're home now."

Wu nodded, wiping tears away from his eyes. "I-I know. Thank you, Mr. Iyu."

Wiping his eyes with the back of his hand, Wu stood on shaky feet.  Iyu helped steady him. He was thinking up a reply when the huge Firryn clapped his shoulder.

"You and I, Snowcloak," she growled as she spun him around to face her, her eyes darting to the sniffling Wu and back to Iyu, "we're gonna have a talk about responsibility."

->IzaldaBridge

-(IzaldaBridge)
Standing a little over a head taller than Iyu was a woman who had more in common with trees than she did with the Firryn.  Packed with muscles barely contained by her armor, Iyu could've been fooled into mistaking her for a short Grynan.

She took a long, scowling, draw of the frigid air and released it. Her eyes intense, she released Iyu from her strong grip and folded her arms across her considerably broad chest.

"Kirrin is a lout. He's a fool and an idiot. But you," she punctuated with a finger prodding into Iyu's chest, "brought a damn pack of Banshee down on Winterhold."

"Far as I'm concerned, you're not welcome here," she growled. With an exhausted grunt she ran her hands through her messy hair. "And you put that kid in danger too; a fucking kid, Snowcloak."

She leaned in, whispering so Wu wouldn't hear. "Natural-born kids like Wu are becoming a rare thing out here. A lot of girls die just attempting birth, understand?"

"But!" Her voice raised back to its normal booming volume, "apparently, I'm to bring you to the village the moment I see you. Even though I'd rather kick your ass out into the snow."

"So, help me understand, Snowcloak Iyu. Why in the name of Kagosho should I even think of letting you in?"

->IzaldaConvo

==IzaldaConvo==
+ {DelgofExcuse == 0} [Delgof wants to meet with me]->DelgofExcuse
+ {WuExcuse == 0} [I promised to return Wu home]->WuExcuse
+ {SaviorExcuse == 0} [I saved this village]->SaviorExcuse
+ [Wu wouldn't have made it on his own] ->WuHelpsOut

-(DelgofExcuse)
"Yeah, that's what he said." She snorts, eyebrows furrowed. "And I say you never arrived," she said, staring daggers at the guard on post.

He shrunk in hrs armor, nodding. "Y-yeah, never saw him."
->IzaldaConvo

-(WuExcuse)
She scoffs. "A fine job you were doing. I think I can take him home from here."

"I don't want to go home with you, I want to go home with Iyu!" Wu grabbed her leg and shook it, scowling.

"You're safer with me than with him. It's what your mother would want."
->IzaldaConvo

-(SaviorExcuse)
"Oh yeah. You sure saved us by bringing a bunch of Banshee in and threatening all our lives."

"In the future, I'd rather you not save us if that's alright with you."
->IzaldaConvo

-(WuHelpsOut)
Wu shrank back as the woman stared at him.

"Oh yeah?" She takes a moment, as if seeing Wu for the first time. "Come to think, who even let you out of the village, Wu?"

Wu stammered, trrying to come up with an excuse. "W-well, I..."

"You went off on your own?" She states it more as a fact than a question. "Did you even tell your mother where you were going?"

"I... she was out hunting and I heard Delgof talking about Iyu and..."

"And you packed up your things, grabbed that crystal your Snowcloak friend here's carrying, and just walked on over to his camp to get him?"

"You left the village, your mom doesn't even know you're gone, and here you are after nearly bearing ripped apart by Banshee - no thanks to this Snowcloak."

She towered over Wu, scowling down at him. "Is that about right?"

Wu looks over to you, pleading silently.

    * [Intervene]
    - "Hey, we're talking about me, remember?" Iyu said, patting off his clothes. "Why you should let me in the village?"
    - The woman turns her head to stare at you. "I still haven't heard a convincing reason."
    - ->FinalSolution
    * [Don't intervene]
    - Iyu shook his head and Wu gritted his teeth, looking down. ->FinalSolution

-(FinalSolution)
Frustrated, Wu yelled, "If you don't let him in, I'll tell everyone about you and Delgof!"
"W-what? What about Delgof?"
"I saw you two alone in the cave last night - I'll tell your husband!"
"T-That's," the woman stammered, "look here, kid, whatever you thought you saw it wasn't--"
"Iyu comes with me or I tell him!"
The woman growled at Wu. "You little sneakthief, you don't tell anyone about that! Understand?" She looked to the guard, who snapped his attention to the horizon, pretending he hadn't heard anything.
Walking up to Iyu, she gave him another harsh stare. A moment of tense silence passed.

"Fine. Fine! Take the kid to his mother's tent but I'll be watching. Then you come right back, understand?"
Her hand tapped the thick rope on her hip. "You take a tour and I'll have your ass out in the snow faster than you can pray to Pyros, Snowcloak."

->ToTheVillage


==ToTheVillage==
#NewPage:village_hub
[They walk in to the village]

[Description of the village proper] #NewPage:village_hub

#NewPage:village_hub
asdf

->DONE
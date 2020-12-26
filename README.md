# Self-Driving-Car-Version-2
Improvement of previous self Driving car

So previously few months ago I made similar simulation were input was raycasts , car velocity , destination coordinate and agent coordinate and the track was mostly straight. Bu that has limitations like the car has to learn what is the shape of road or you can say map of the road . So suppose I just shift the simulation's coordinate a little bit not the car but entire simulation's coordinate is changed so now for this new coordinate our agent is not trained but for use our training simulation looks same. so to overcome this I thought I don't want my agent to memorize entire road it can just use a GPS system to know where it has to turn. Like in GPS navigation if we don't know the city but we know how to navigate using GPS we can go to our goal. So I searched how to create a navigation system , then I came acroos Brackeys youtube navmesh tutorial so there i find that I can use navmesh to calculate path long story short I worked a lot of hit n trials then finally able to create a navigation system. I removed every unneccesary things from image and just put agent at the center and a pink line as highlight of the direction to turn. I gave it raycasts to detect obstacles. It resets with negative rewrad if it bumps into a car or anything. Velocity of car was also a input. So now it still don't know if it has reached the destination I mean there is nothing mentioned like distance. What i did was instead of giving value of the distance I gave input as 1 if current distance between agent and goal is less than previous frame other wise -1.  it has to undertand that it is going away from goal if  one of it's input is -1 and it is getting closer to goal if it is +1.

if it goes forward and it's distance is getting smaller i.e. one of it's input is +1 if both this condition is true then I gave it small positive reward other small negative reward.

collision has large penalty and resets the env

reaching goal gives large positive reward and resets

In GPS system u can see I used black as background because I thought it has value 0 so it wont' affect much

pink highlight is just a missing material and I was being lazy because I had already trained it a little bit so model won't work

I trained at first with 2 raycast in forward direction which worked as long as the cars were facing the direction of path but when I made them face opposite then they were unable to u turn

but it was already trained alot so i had start over then I again tried to train it by putting cars facing all directions so that they will learn but it didn't work because they did not have raycasts in their back so they couldn't take reverse and detect obstacle

they tried to take reverse some of them did not nedd to take reverse they could just take extreme turn and they were facing the path these type of agents were working

but at last I gave them raycast in all direction so that they could detect obastacle in the back and yep in the video you can see that some them took a nice reverse and took a u turn

i did not expect it to do that i thought it can face the path by just taking hard turn

it gives negative reward for going backward but one them learnt the trick

This project is designed to show that a fully functional Self-Driving car, that
can reach to it’s destination and avoid obstacles, can be built with least
amount of inputs so that it’s training is fast.
This model is hoped to be extendable to real life remote controlled small
cars.
Since it uses an image for navigation It can be replaced with GPS system
that highlights the path to take, it’s ray cast can be replaced with Arduino
sonar sensors that detects distance of the obstacles and another sensor to
detect its velocity.
This may require little bit processing but the same model can be used to run
that car.

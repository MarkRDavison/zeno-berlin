## Pages

***
### Per user
#### Dashboard
This is a summary of the last updated stories
	-	Can navigate to a specific story to view history/manage it

#### Manage list
This is a grid (paginated) of all stories
	-	Can filter by author
		-	Maybe more, but depends on complexity of persisted story info
	-	Can navigate to a specific story to view history/manage it

#### Manage individual
This is for a specific story
	-	Can see the update history
	-	It may guess at the next upcoming update???
	-	Can remove it or change the update type

#### Individual settings
This allows users to edit settings
	-	Default update type
	-	Export stories
		-	Option to export updates as well
	
***
### Admin
#### Stats
This page show totals aggregated over all users
	-	Can see how many requests being made
		-	Group by site etc?

#### Site settings
Edit site wide settings, all admins see the same
	-	Enabled/supported sites
	-	Set maximums etc, update batch sizes???

***
### Anonymous
#### Description/stats???
Some random anonymous/landing page so you can be on the site and not immediately get redirected to auth????


***
***
***

## Features
Series, ao3 concept does ffnet have it? maybe if site doesnt support it you can manually add them??
Importing needs to be an async job

Date time local vs utc

With the visual hint for unread chapters, have a user setting as to whether this is just as expected, or respects the update type

***
## Next

## MISC
Queries need to use ValidateAndProcessQueryHandler???
Method for a fandom to be auto assigned as a parent for a new fandom? Wildcard/rules etc
Consistency around naming of bool IsComplete vs Complete etc
Story summary???
If you are using the site while the cron job runs, stuff may be updated in the background, is this an issue???
shared action/response are public, non shared ones that are same as shared, internal, keep within reducer/effect???

On the author/fandom pages parent author/fandom is not taken into account when filtering stories to show

support for backfilling missing data?
 - e.g. chapter/story update address/title
	- We do this if we've skipped over updates, but we dont fill them in from chatper 1 onwards
	- Maybe do the same process, but use the published date as the chapter 1 date, then either evenly or stuff all the updates on publish/update date??

## BUGS

UNSUPPORTED_SITE returned with http if site has https recorded, strip protocol out???
navigating to /fandoms after initial load does not load them, so newly added stories wont have their fandoms show up till reload

duplicate story gives no response exit in the console.  Message bar on modal??? or use snackbar


## Bigger things
Get proper auth going
 - still need to fix having to up the proxy buffer size, use session to store tokens instead of putting token in cookie



### DONE

## Features

Fandom
 - a way to manage this?
 - some stories have like 50, maybe a management page per user/site???
	- Each fandom can have a parent associated, so 'Star Wars - All Media Types', 'Star Wars: Rebels' etc all are summarized as Star Wars? What about cross overs???

DEPLOY TO K8S

Authors

A way to manually add updates

Stories that update multiple times at once?
 - Manually add missing updates so if chapter 22&23 at once, add  both updates at 23's update date.  But only notify of 23 and say 2! new chapters???
 
 - Some concept of marking where you last were at??? So like opt in and say you've read up to chapter X, then when there are new updates it tells you how many you have to read, you can then set a new value/mark it read up to current etc???
	- Icon next to favourite??? hit it when you are up to date, and if it has been set but not currently at the current chapter then show some visual hint

 - A way to add the update type to the stories, maybe on add as well as manage
	- Make the check stories process respect update type
 - Story updates record the chapter title and address
	- Can link to a specific chapter now

Async background job processing
 - Runs as a separate container, can scale up
 - Subscribes to redis for notifications on checking for jobs
 - Reads from database for jobs
	- Deconflicts with other job containers to not duplicate
 - Remove the cron jobs from the api move them to the jobs api
	- Need coordination to not duplicate job creation
	    - Have ANOTHER container, that doesnt get duplicates, orcherstrator? runs cron, create jobs???
 - Create docker files for job runner and job orchestrator
 - Create K8s yamls for new containers
	- remove existing params for old containers
	- add redis to all etc???
 - trial scaling api/job runner containers :)

Investigate distributed pub sub or is redis fine for this???
Get rid of IRepository, investigate dbcontext and transactions but use a prebuilt solution

## BUGS

Duplicated fandoms
 - FandomService.RetrieveFandomByExternalName may be failing when running in a batch???
   - Use Cache instead???

bffroot retrieval is bad, use app settings and sed command into there???

The stylesheet https://fanfic.markdavison.kiwi/css/berlin.min.css was not loaded because its MIME type, "text/html", is not "text/css".
 - nginx catch all is messing with this??? but it is still loading

Hard refresh of anything but root gives nginx 404
checking a story that doesnt have an author does not set it on the story, it does create the author
Trying to add an update before the first one currently recorded doesnt work
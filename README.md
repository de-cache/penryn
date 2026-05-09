# penryn

static site generator with some tricks

## what is it

the goal is to be a static site generator that easily accomodates for a full-stack CMS experience with both API routes
and a dashboard for creating new posts.

## how to use

1. download git repo
2. build the project
3. run ``penryn new <project name, defaults to 'my-penryn-project'>``
4. move into your project folder and run ``penryn build``
5. check out public folder and view the result

## extra stuff

* your config file can be parsed into your documents using ``{{ project.[property] }}``
# slowik

Słowik - profil wyrazów w zadanych korpusach


## How to use:

* (optional) Sending corpus guid as confirmation of processing via email:

   Make `.env` file with gmail credentials (EMIAL, EMAILPASSWORD) to gmail account which accept *''Less secure app access''*.
   
   Example of `.env` file:
      
      EMAIL=exmpl@exmpl.exmpl
      EMAILPASSWORD=3xMp1!

Prepare and run images:

$ docker-compose up

### Other comments

The `docker-entrypoint.sh` script has to be saved with the **LF** end of line sequence.